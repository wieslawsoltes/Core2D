// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    /// <summary>
    /// 
    /// </summary>
    public class DxfSpline : DxfObject
    {
        /// <summary>
        /// 
        /// </summary>
        public string Layer { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Color { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfVector3 NormalVector { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfSplineFlags SplineFlags { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int SplineCurveDegree { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double KnotTolerance { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double ControlPointTolerance { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double FitTolerance { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfVector3 StartTangent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfVector3 EndTangent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double[] Knots { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double[] Weights { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfVector3[] ControlPoints { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DxfVector3[] FitPoints { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="version"></param>
        /// <param name="id"></param>
        public DxfSpline(DxfAcadVer version, int id)
            : base(version, id)
        {
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void Defaults()
        {
            NormalVector = new DxfVector3(0.0, 0.0, 1.0);
            SplineFlags = DxfSplineFlags.Planar;
            SplineCurveDegree = 3;
            
            KnotTolerance =  0.0000001;
            ControlPointTolerance = 0.0000001;
            FitTolerance = 0.0000000001;
            
            StartTangent = default(DxfVector3);
            EndTangent = default(DxfVector3);
            
            Knots = new double[8];
            //Knots[0] = 0.0;
            //Knots[1] = 0.0;
            //Knots[2] = 0.0;
            //Knots[3] = 0.0;
            //Knots[4] = 1.0;
            //Knots[5] = 1.0;
            //Knots[6] = 1.0;
            //Knots[7] = 1.0;
            
            Weights =  null;
   
            ControlPoints = new DxfVector3[4];
            //ControlPoints[0] = new DxfVector3(0.0, 0.0, 0.0);
            //ControlPoints[1] = new DxfVector3(0.0, 0.0, 0.0);
            //ControlPoints[2] = new DxfVector3(0.0, 0.0, 0.0);
            //ControlPoints[3] = new DxfVector3(0.0, 0.0, 0.0);
            
            FitPoints = default(DxfVector3[]);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string Create()
        {
            Reset();

            Add(0, DxfCodeName.Spline);

            Entity();

            if (Version > DxfAcadVer.AC1009)
                Subclass(DxfSubclassMarker.Spline);
            
            Add(8, Layer);
            Add(62, Color);

            if (SplineFlags.HasFlag(DxfSplineFlags.Planar))
            {
                Add(210, NormalVector.X);
                Add(220, NormalVector.Y);
                Add(230, NormalVector.Z);
            }

            Add(70, (int)SplineFlags);
            Add(71, SplineCurveDegree);
            Add(72, Knots != null ? Knots.Length : 0);
            Add(73, ControlPoints != null ? ControlPoints.Length : 0);
            Add(74, FitPoints != null ? FitPoints.Length : 0);
            
            Add(42, KnotTolerance);
            Add(43, ControlPointTolerance);
            Add(44, FitTolerance);
            
            if (StartTangent != null)
            {
                Add(12, StartTangent.X);
                Add(22, StartTangent.Y);
                Add(32, StartTangent.Z);
            }

            if (EndTangent != null)
            {
                Add(13, EndTangent.X);
                Add(23, EndTangent.Y);
                Add(33, EndTangent.Z);
            }

            if (Knots != null)
            {
                foreach (var knot in Knots) 
                {
                    Add(40, knot);
                }
            }

            if (Weights != null)
            {
                foreach (var weight in Weights) 
                {
                    Add(41, weight);
                }
            }

            if (ControlPoints != null)
            {
                foreach (var cp in ControlPoints) 
                {
                    Add(10, cp.X);
                    Add(20, cp.Y);
                    Add(30, cp.Z);
                }
            }

            if (FitPoints != null)
            {
                foreach (var fp in FitPoints) 
                {
                    Add(11, fp.X);
                    Add(21, fp.Y);
                    Add(31, fp.Z);
                }
            }

            return Build();
        }
    }
}
