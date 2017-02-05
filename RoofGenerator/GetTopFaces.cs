//using RevitServices.Persistence;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using DynamoServices;
//using Dyn = Revit.Elements;
//using DynApp = Revit.Application;
//using DynGeo = Autodesk.DesignScript.Geometry;
//using Revit.GeometryObjects;
//using Autodesk.Revit.DB;
//using Autodesk.DesignScript.Runtime;
//using Autodesk.Revit.DB.IFC;
//using Revit.GeometryConversion;

//namespace OnDynamoUtils
//{

//    static public partial class GetFaces
//    {
//        [CanUpdatePeriodically(false)]
//        static public IList<DynGeo.Surface> GetTopFaces(Dyn.Element floorElement)
//        {
//            IList<DynGeo.Surface> DynamoFaces = new List<DynGeo.Surface>();
//            Document doc = DocumentManager.Instance.CurrentDBDocument;

//            CheckForErrors(floorElement);

//            Floor currentFloor = CheckForErrors(floorElement);
//            IList<Reference> topFacesRef = HostObjectUtils.GetTopFaces(currentFloor);
//            IList<PlanarFace> topFaces = new List<PlanarFace>();

//            IsListOfPlanarFaces(topFacesRef, currentFloor, out topFaces);

//            ConvertRevitFacesIntoDynamoFaces(DynamoFaces, topFaces);

//            return DynamoFaces;
//        }

//        [CanUpdatePeriodically(false)]
//        static public IList<DynGeo.Surface> GetBottomFaces(Dyn.Element floorElement)
//        {
//            IList<DynGeo.Surface> DynamoFaces = new List<DynGeo.Surface>();
//            Document doc = DocumentManager.Instance.CurrentDBDocument;

//            CheckForErrors(floorElement);

//            Floor currentFloor = CheckForErrors(floorElement);
//            IList<Reference> bottomFacesRef = HostObjectUtils.GetBottomFaces(currentFloor);
//            IList<PlanarFace> bottomFaces = new List<PlanarFace>();

//            IsListOfPlanarFaces(bottomFacesRef, currentFloor, out bottomFaces);

//            ConvertRevitFacesIntoDynamoFaces(DynamoFaces, bottomFaces);

//            return DynamoFaces;
//        }

//        private static Floor CheckForErrors(Dyn.Element targetElement)
//        {
//            if (targetElement == null)
//                throw new Exception("Error, Selected element is invalid.");

//            Floor currentFloor = targetElement.InternalElement as Floor;

//            if (currentFloor == null)
//                throw new Exception("Error, Selected element is not a floor.");
//            return currentFloor;
//        }

//        private static void ConvertRevitFacesIntoDynamoFaces(IList<DynGeo.Surface> DynamoFaces, IList<PlanarFace> topFaces)
//        {
//            foreach (PlanarFace currentPlanarFace in topFaces)
//            {
//                DynGeo.Surface currentReference = RevitToProtoFace.ToProtoType(currentPlanarFace, true, null).ElementAt(0);
//                DynamoFaces.Add(currentReference);
//            }
//        }

//        static private bool IsListOfPlanarFaces(IList<Reference> targetListOfReferences, Element currentElement, out IList<PlanarFace> targetListOfPlanarFaces)
//        {
//            targetListOfPlanarFaces = new List<PlanarFace>();
//            foreach (Reference currentReference in targetListOfReferences)
//            {
//                Face currentFace = GetFaceFromReference(currentReference, currentElement);
//                PlanarFace currentPlanarFace = null;
//                if (currentFace != null)
//                {
//                    currentPlanarFace = currentFace as PlanarFace;
//                    if (currentPlanarFace != null)
//                        targetListOfPlanarFaces.Add(currentPlanarFace);
//                    else
//                        return false;
//                }
//            }
//            return true;
//        }

//        static private Face GetFaceFromReference(Reference targetReference, Element targetElement)
//        {
//            Face currentFace = null;
//            if (targetReference != null && targetElement != null)
//            {
//                GeometryObject currentGeometryObject = targetElement.GetGeometryObjectFromReference(targetReference);
//                if (currentGeometryObject != null)
//                {
//                    if (currentGeometryObject is Face)
//                        currentFace = currentGeometryObject as Face;
//                }
//            }

//            return currentFace;
//        }

//    }
//}
