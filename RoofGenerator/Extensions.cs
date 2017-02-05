using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoofGenerator
{
    static public class Extensions
    {

        static internal bool IsAlmostEqualTo(this Curve firstCurve, Curve secondCurve)
        {
            if (firstCurve != null && secondCurve != null)
            {
                XYZ firstCurveFirstPoint = firstCurve.GetEndPoint(0);
                XYZ firstCurveSecondPoint = firstCurve.GetEndPoint(1);

                XYZ secondCurveFirstPoint = secondCurve.GetEndPoint(0);
                XYZ secondCurveSecondPoint = secondCurve.GetEndPoint(1);

                if (firstCurveFirstPoint.IsAlmostEqualTo(secondCurveFirstPoint, 0.02) && firstCurveSecondPoint.IsAlmostEqualTo(secondCurveSecondPoint, 0.02) ||
                    firstCurveFirstPoint.IsAlmostEqualTo(secondCurveSecondPoint, 0.02) && firstCurveSecondPoint.IsAlmostEqualTo(secondCurveFirstPoint, 0.02))
                    return true;
            }
            return false;
        }

        internal static bool IsAlmostEqualTo(this double firstNumber, double secondNumber, double tolerance = 0.01)
        {
            double diference = Math.Abs(firstNumber - secondNumber);

            if (diference <= tolerance)
            {
                return true;
            }

            return false;
        }

        internal static bool ContainsSimilarCurve(this IList<Curve> currentList, Curve curve)
        {
            if (currentList != null)
            {
                foreach (Curve currentCurve in currentList)
                {
                    if (currentCurve.IsAlmostEqualTo(curve))
                        return true;
                }
            }
            return false;
        }
    }
}
