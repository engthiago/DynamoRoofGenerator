using Autodesk.Revit.DB;
using Autodesk.Revit.DB.IFC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dyn = Revit.Elements;

namespace RoofGenerator
{

    enum RoofLineType { Hip, Ridge, Valley, Eave, Undefined };

    struct curveInfo
    {
        public Curve curve;
        public RoofLineType roofLineType;
    }

    static public partial class RoofGenerator
    {

        static internal class RoofStorage
        {
            static internal Autodesk.Revit.ApplicationServices.Application revitApp;
            static internal bool isRegister = false;

            static internal string licensePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\onBimBox\\";
            static internal string dllVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            static internal string licenseFile = licensePath + "lic" + dllVersion + ".lc";

            static internal string roofGeneratorVersion = "";
        }

        static private IList<XYZ> GetOrganizedPoints(PlanarFace targetPlanarFace)
        {
            IList<XYZ> listOfPoints = new List<XYZ>();
            if (targetPlanarFace != null)
            {
                CurveLoop curentCurveLoop = GetOuterCurveLoop(targetPlanarFace);
                XYZ planeOrigin = targetPlanarFace.Origin;
                Curve currentCurve = null;

                for (int i = 0; i < curentCurveLoop.Count(); i++)
                {
                    if (currentCurve == null)
                        currentCurve = curentCurveLoop.Where(c => c.GetEndPoint(0).Z.IsAlmostEqualTo(planeOrigin.Z) && c.GetEndPoint(1).Z.IsAlmostEqualTo(planeOrigin.Z)).FirstOrDefault();
                    else
                        currentCurve = curentCurveLoop.Where(c => c is Curve).OrderBy(c => c.GetEndPoint(0).DistanceTo(currentCurve.GetEndPoint(1))).FirstOrDefault();

                    listOfPoints.Add(currentCurve.GetEndPoint(0));
                }
            }
            return listOfPoints;
        }

        static private CurveLoop GetOuterCurveLoop(PlanarFace targetFace)
        {
            CurveLoop currentCurveLoop = new CurveLoop();

            if (targetFace != null)
            {
                IList<XYZ> Points = new List<XYZ>();
                IList<IList<CurveLoop>> currentListOfListOfCurveLoops = ExporterIFCUtils.SortCurveLoops(targetFace.GetEdgesAsCurveLoops());

                if (currentListOfListOfCurveLoops != null)
                {
                    if (currentListOfListOfCurveLoops.Count > 0)
                    {
                        IList<CurveLoop> currentOuterLoop = currentListOfListOfCurveLoops[0];
                        if (currentOuterLoop != null)
                        {
                            if (currentOuterLoop.Count > 0)
                                currentCurveLoop = currentOuterLoop[0];
                        }
                    }

                }
            }
            return currentCurveLoop;
        }

        static private IList<XYZ> GetCurveNotDuplicatedPoints(CurveLoop targetCurveLoop)
        {
            IList<XYZ> curvePoints = new List<XYZ>();
            if (targetCurveLoop != null)
            {
                foreach (Curve currentCurve in targetCurveLoop)
                {
                    curvePoints.Add(currentCurve.GetEndPoint(0));
                }
            }
            return curvePoints;
        }

        static private bool IsListOfPlanarFaces(IList<Reference> targetListOfReferences, Element currentElement, out IList<PlanarFace> targetListOfPlanarFaces)
        {
            targetListOfPlanarFaces = new List<PlanarFace>();
            foreach (Reference currentReference in targetListOfReferences)
            {
                Face currentFace = GetFaceFromReference(currentReference, currentElement);
                PlanarFace currentPlanarFace = null;
                if (currentFace != null)
                {
                    currentPlanarFace = currentFace as PlanarFace;
                    if (currentPlanarFace != null)
                        targetListOfPlanarFaces.Add(currentPlanarFace);
                    else
                        return false;
                }
            }
            return true;
        }

        static private Face GetFaceFromReference(Reference targetReference, Element targetElement)
        {
            Face currentFace = null;
            if (targetReference != null && targetElement != null)
            {
                GeometryObject currentGeometryObject = targetElement.GetGeometryObjectFromReference(targetReference);
                if (currentGeometryObject != null)
                {
                    if (currentGeometryObject is Face)
                        currentFace = currentGeometryObject as Face;
                }
            }

            return currentFace;
        }

        static private IList<FamilySymbol> ConvertDynFamilyTypeListToRevitFamilySymbolList(IList<Dyn.FamilyType> targetListOfFamilyTypes)
        {
            IList<FamilySymbol> revitFamilySymbolList = new List<FamilySymbol>();
            if (targetListOfFamilyTypes != null)
            {
                foreach (Dyn.FamilyType currentFamily in targetListOfFamilyTypes)
                {
                    FamilySymbol currentSymbol = currentFamily.InternalElement as FamilySymbol;
                    if (currentSymbol != null)
                    {
                        if (AdaptiveComponentInstanceUtils.IsAdaptiveFamilySymbol(currentSymbol))
                            revitFamilySymbolList.Add(currentSymbol);
                    }
                }
            }
            return revitFamilySymbolList;
        }

        static private void CreateAdaptiveComponent(Document doc, PlanarFace currentFace, FamilySymbol currentFamilySymbol)
        {
            FamilyInstance instance = AdaptiveComponentInstanceUtils.CreateAdaptiveComponentInstance(doc, currentFamilySymbol);
            IList<ElementId> placePointIds = new List<ElementId>();
            placePointIds = AdaptiveComponentInstanceUtils.GetInstancePlacementPointElementRefIds(instance);

            IList<XYZ> placementPoints = GetOrganizedPoints(currentFace);
            int pointPlace = 0;
            // Set the position of each placement point
            foreach (ElementId id in placePointIds)
            {
                ReferencePoint point = doc.GetElement(id) as ReferencePoint;
                point.Position = placementPoints[pointPlace];
                pointPlace++;
            }
        }

        static private void CreateAdaptiveComponentFromLine(Document doc, Curve targetLine, FamilySymbol currentFamilySymbol)
        {
            if (targetLine != null)
            {
                if (AdaptiveComponentFamilyUtils.GetNumberOfPlacementPoints(currentFamilySymbol.Family) == 2)
                {
                    FamilyInstance instance = AdaptiveComponentInstanceUtils.CreateAdaptiveComponentInstance(doc, currentFamilySymbol);
                    IList<ElementId> placePointIds = new List<ElementId>();
                    placePointIds = AdaptiveComponentInstanceUtils.GetInstancePlacementPointElementRefIds(instance);

                    XYZ firstPoint = null;
                    XYZ secondPoint = null;

                    if (targetLine.GetEndPoint(0).Z < targetLine.GetEndPoint(1).Z)
                    {
                        firstPoint = targetLine.GetEndPoint(0);
                        secondPoint = targetLine.GetEndPoint(1);
                    }
                    else
                    {
                        firstPoint = targetLine.GetEndPoint(1);
                        secondPoint = targetLine.GetEndPoint(0);
                    }

                    ReferencePoint rFirstPoint = doc.GetElement(placePointIds[0]) as ReferencePoint;
                    ReferencePoint rSecondPoint = doc.GetElement(placePointIds[1]) as ReferencePoint;

                    rFirstPoint.Position = firstPoint;
                    rSecondPoint.Position = secondPoint;
                }
            }
        }

        static private IList<Curve> GetListOfRidgesHipsAndValleys(IList<PlanarFace> targetPlanarFaceList)
        {
            IList<Curve> listOfCurves = new List<Curve>();
            if (targetPlanarFaceList != null)
            {
                foreach (PlanarFace currentFace in targetPlanarFaceList)
                {
                    XYZ fOrigin = currentFace.Origin;
                    IList<Curve> eaveCurves = GetOuterCurveLoop(currentFace).Where(c => c.GetEndPoint(0).Z.IsAlmostEqualTo(fOrigin.Z) && c.GetEndPoint(1).Z.IsAlmostEqualTo(fOrigin.Z)).ToList();
                    Curve eaveCurve = null;

                    if (eaveCurves.Count() > 0)
                        eaveCurve = eaveCurves[0];

                    if (currentFace != null)
                    {
                        CurveLoop currentFaceOuterLoop = GetOuterCurveLoop(currentFace);

                        foreach (Curve currentCurve in currentFaceOuterLoop)
                        {
                            if (!listOfCurves.ContainsSimilarCurve(currentCurve))
                            {
                                if (!currentCurve.IsAlmostEqualTo(eaveCurve))
                                    listOfCurves.Add(currentCurve);
                            }
                        }
                    }
                }
            }
            return listOfCurves;
        }

        static private curveInfo GetCurveInformation(Element targetRoof, Curve targetCurve, IList<PlanarFace> targetPlanarFaceList)
        {
            if (targetPlanarFaceList != null)
            {
                foreach (PlanarFace currentPlanarFace in targetPlanarFaceList)
                {
                    EdgeArrayArray EdgeLoops = currentPlanarFace.EdgeLoops;
                    foreach (EdgeArray currentEdgeArray in EdgeLoops)
                    {
                        foreach (Edge currentEdge in currentEdgeArray)
                        {
                            if (currentEdge != null)
                            {
                                Curve edgeCurve = currentEdge.AsCurve();
                                if (edgeCurve.IsAlmostEqualTo(targetCurve))
                                {
                                    if (edgeCurve.GetEndPoint(0).Z.IsAlmostEqualTo(currentPlanarFace.Origin.Z) && edgeCurve.GetEndPoint(1).Z.IsAlmostEqualTo(currentPlanarFace.Origin.Z))
                                    {
                                        return new curveInfo { curve = edgeCurve, roofLineType = RoofLineType.Eave };

                                    }
                                    else if (edgeCurve.GetEndPoint(0).Z.IsAlmostEqualTo(currentPlanarFace.Origin.Z) || edgeCurve.GetEndPoint(1).Z.IsAlmostEqualTo(currentPlanarFace.Origin.Z))
                                    {
                                        PlanarFace firstFace = currentEdge.GetFace(0) as PlanarFace;
                                        PlanarFace secondFace = currentEdge.GetFace(1) as PlanarFace;

                                        if (!targetPlanarFaceList.Contains(firstFace) || !targetPlanarFaceList.Contains(secondFace))
                                        {
                                            return new curveInfo { curve = edgeCurve, roofLineType = RoofLineType.Hip };
                                        }
                                        else
                                        {
                                            if (GetOuterCurveLoop(firstFace).Count() == 3 || GetOuterCurveLoop(secondFace).Count() == 3)
                                            {
                                                return new curveInfo { curve = edgeCurve, roofLineType = RoofLineType.Hip };
                                            }
                                            else
                                            {
                                                XYZ startingPoint = edgeCurve.GetEndPoint(0).Z < edgeCurve.GetEndPoint(1).Z ? edgeCurve.GetEndPoint(0) : edgeCurve.GetEndPoint(1);

                                                XYZ extendedPoint = GetExtendedPoint(startingPoint, firstFace);
                                                XYZ rayTracePoint = new XYZ(extendedPoint.X, extendedPoint.Y, extendedPoint.Z + 999);

                                                ReferenceIntersector ReferenceIntersect = new ReferenceIntersector(targetRoof.Id, FindReferenceTarget.Element, (targetRoof.Document.ActiveView as View3D));
                                                ReferenceWithContext RefContext = ReferenceIntersect.FindNearest(rayTracePoint, XYZ.BasisZ.Negate());

                                                if (RefContext == null)
                                                    return new curveInfo { curve = edgeCurve, roofLineType = RoofLineType.Hip };

                                                return new curveInfo { curve = edgeCurve, roofLineType = RoofLineType.Valley };
                                            }
                                        }
                                    }
                                    else
                                    {
                                        return new curveInfo { curve = edgeCurve, roofLineType = RoofLineType.Ridge };
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return new curveInfo { curve = targetCurve, roofLineType = RoofLineType.Undefined };
        }

        static private XYZ GetExtendedPoint(XYZ startPosition, PlanarFace targetPlanarFace, double amount = 0.1)
        {
            if (targetPlanarFace != null)
            {
                CurveLoop currentCurveLoop = GetOuterCurveLoop(targetPlanarFace);

                foreach (Curve currentCurve in currentCurveLoop)
                {
                    if (currentCurve.GetEndPoint(0).Z.IsAlmostEqualTo(targetPlanarFace.Origin.Z) && currentCurve.GetEndPoint(1).Z.IsAlmostEqualTo(targetPlanarFace.Origin.Z))
                    {
                        XYZ p0 = startPosition;
                        XYZ p1 = startPosition;

                        if (p0.IsAlmostEqualTo(currentCurve.GetEndPoint(0)))
                            p1 = currentCurve.GetEndPoint(1);
                        else
                            p1 = currentCurve.GetEndPoint(0);

                        XYZ lineDirection = Line.CreateBound(p0, p1).Direction;
                        XYZ extendendPoint = p0 + (lineDirection.Negate() * amount);

                        return extendendPoint;
                    }
                }
            }
            return startPosition;
        }

        //Not currently being used

        static private void CreateRoofPoints(Document doc, IList<PlanarFace> targetPlanarFaceList, FamilySymbol targetFamilySymbol)
        {
            if (targetPlanarFaceList != null)
            {
                foreach (PlanarFace currentPlanarFace in targetPlanarFaceList)
                {
                    CurveLoop currentLoop = GetOuterCurveLoop(currentPlanarFace);
                    if (GetOuterCurveLoop(currentPlanarFace).Count() == 3)
                    {
                        //doc.Create.NewFamilyInstance(currentPlanarFace.Origin, targetFamilySymbol, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);

                        XYZ planeOrigin = currentPlanarFace.Origin;
                        Curve currentCurve = null;

                        for (int i = 0; i < currentLoop.Count(); i++)
                        {
                            if (currentCurve == null)
                                currentCurve = currentLoop.Where(c => c.GetEndPoint(0).Z.IsAlmostEqualTo(planeOrigin.Z) && c.GetEndPoint(1).Z.IsAlmostEqualTo(planeOrigin.Z)).FirstOrDefault();
                            else
                                currentCurve = currentLoop.Where(c => c is Curve).OrderBy(c => c.GetEndPoint(0).DistanceTo(currentCurve.GetEndPoint(1))).FirstOrDefault();

                            doc.Create.NewFamilyInstance(currentCurve.GetEndPoint(0), targetFamilySymbol, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                        }

                        //IList<XYZ> InsertionPoints = GetRoofInsertionPoints(currentPlanarFace);
                        //if (InsertionPoints.Count > 0)
                        //{
                        //    doc.Create.NewFamilyInstance(InsertionPoints[0], targetFamilySymbol, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                        //    XYZ midPoint = (InsertionPoints[0] + InsertionPoints[1]) / 2;
                        //    doc.Create.NewFamilyInstance(midPoint, targetFamilySymbol, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                        //}
                    }
                }
            }
        }

        static private IList<XYZ> GetRoofInsertionPoints(IList<PlanarFace> targetPlanarFaceList)
        {
            IList<XYZ> PointsList = new List<XYZ>();
            if (targetPlanarFaceList != null)
            {
                foreach (PlanarFace currentPlanarFace in targetPlanarFaceList)
                {
                    CurveLoop currentCurveLoop = GetOuterCurveLoop(currentPlanarFace);
                    PointsList = PointsList.Union(GetCurveNotDuplicatedPoints(currentCurveLoop)).ToList();
                }
            }
            return PointsList;
        }

        static private IList<XYZ> GetRoofInsertionPoints(PlanarFace targetPlanarFace)
        {
            IList<XYZ> PointsList = new List<XYZ>();

            if (targetPlanarFace != null)
            {
                CurveLoop currentCurveLoop = GetOuterCurveLoop(targetPlanarFace);
                PointsList = PointsList.Union(GetCurveNotDuplicatedPoints(currentCurveLoop)).ToList();
            }

            return PointsList;
        }

    }
}

