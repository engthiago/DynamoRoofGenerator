using RevitServices.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamoServices;
using Dyn = Revit.Elements;
using DynApp = Revit.Application;
using Autodesk.Revit.DB;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB.IFC;
using Autodesk.Revit.UI;

namespace RoofGenerator
{

    static public partial class RoofGenerator
    {
        [CanUpdatePeriodically(false)]
        static public string CreateRoof(Dyn.Element roofElement, IList<Dyn.FamilyType> listOfFamilyTypes, Dyn.FamilyType ridgeFamilyType)
        {
            Document doc = DocumentManager.Instance.CurrentDBDocument;
            RoofStorage.revitApp = doc.Application;

            FootPrintRoof currentRoof = roofElement.InternalElement as FootPrintRoof;

            if (currentRoof == null)
                throw new Exception("Error, element is not a footprint roof.");

            Parameter selectedRoofSlopeParameter = currentRoof.get_Parameter(BuiltInParameter.ROOF_SLOPE);
            double selectedRoofSlope = selectedRoofSlopeParameter.AsDouble();

            //Verify if the roof has all the same slopes
            if (selectedRoofSlope <= 0)
            {
                throw new Exception("Error, it was not possible to estabilish a unique slope for the roof, please, make sure all eaves have the same slope.");
            }
            //Verify if the roof has the minimal slope
            if (selectedRoofSlope < 0.098)
            {
                throw new Exception("Error, please make sure the roof has a minimum of 10% slope.");
            }

            IList<Reference> faceRefList = HostObjectUtils.GetTopFaces(currentRoof);
            IList<PlanarFace> planarFaceList = new List<PlanarFace>();

            if (!IsListOfPlanarFaces(faceRefList, currentRoof, out planarFaceList))
                throw new Exception("Error, invalid roof selected, please make sure all faces of the roof are flat.");

            FamilySymbol ridgeFamilySymbol = ridgeFamilyType.InternalElement as FamilySymbol;

            if (ridgeFamilySymbol == null)
                throw new Exception("Error, ridgeFamilyType is not a valid Family Type.");

            if (!AdaptiveComponentFamilyUtils.IsAdaptiveComponentFamily(ridgeFamilySymbol.Family))
                throw new Exception("Error, ridgeFamilyType is not a valid Adaptive component type");

            if (AdaptiveComponentFamilyUtils.GetNumberOfPlacementPoints(ridgeFamilySymbol.Family) != 2)
                throw new Exception("Error, ridgeFamilyType must have two insertion points");

            IList<FamilySymbol> listOfFamilySymbols = ConvertDynFamilyTypeListToRevitFamilySymbolList(listOfFamilyTypes);

            if (listOfFamilySymbols.Count() < 1)
                throw new Exception("Error, no adaptive family types were found on the input list");

            if (doc.ActiveView as View3D == null)
                throw new Exception("Error, please use this command on a 3d view");

            //currentPointSymbol.Activate();
            string results = "";

            results += "Number of Roof Instances " + planarFaceList.Count() + "\n";

            foreach (PlanarFace currentFace in planarFaceList)
            {
                int numberOfCurves = GetOuterCurveLoop(currentFace).Count();

                FamilySymbol currentFamilySymbol = listOfFamilySymbols.Where(fs => AdaptiveComponentFamilyUtils.GetNumberOfPlacementPoints(fs.Family) == numberOfCurves).FirstOrDefault();

                if (currentFamilySymbol == null)
                    continue;

                CreateAdaptiveComponent(doc, currentFace, currentFamilySymbol);
            }

            IList<Curve> ridgeCurves = GetListOfRidgesHipsAndValleys(planarFaceList);
            results += "Number of Ridge Instances " + ridgeCurves.Count();

            foreach (Curve currentCurve in ridgeCurves)
            {
                curveInfo curvI = GetCurveInformation(currentRoof, currentCurve, planarFaceList);

                if (curvI.roofLineType == RoofLineType.Ridge || curvI.roofLineType == RoofLineType.Hip)
                {
                    CreateAdaptiveComponentFromLine(doc, currentCurve, ridgeFamilySymbol);
                }
            }


            return results;
        }
    }
}
