/* 
 * Created by Alex 'Extravert' Kasaurov
 * From  xgm.guru community
 */
using System.Collections.Generic;
using UnityEngine;

namespace XGM.GURU
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class WallConstructor : MonoBehaviour
    {
        public float height = 5;
        public float uvSize = 1f;
        public float deep = 0.5f;
        public bool updateMode;
        public float smoothDistance = 1f;

        public void OnDrawGizmos()
        { 
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position + transform.right, transform.position - transform.right);
            Gizmos.DrawLine(transform.position + transform.forward, transform.position - transform.forward);

            if (transform.childCount < 2)
                return;

            Gizmos.color = new Color(1, 1, 1, 0.5f);
            for (int i = 0; i < transform.childCount - 1; i++)
                Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);

           
            for (int i = 0; i < bBottom.Count - 1; i++)
            {
                Gizmos.color = updateMode ? Color.red : Color.blue;
                Gizmos.DrawLine(transform.position + transform.rotation*aBottom[i],
                    transform.position + transform.rotation*aBottom[i + 1]);
                Gizmos.color = updateMode ? Color.red : Color.yellow;
                Gizmos.DrawLine(transform.position + transform.rotation * bBottom[i],
                    transform.position + transform.rotation * bBottom[i + 1]);
            }

            if (updateMode)
                Calculate();
        }

        public List<Vector3> aBottom = new List<Vector3>(),
                             bBottom = new List<Vector3>(),
                             aTop = new List<Vector3>(),
                             bTop = new List<Vector3>();

        public void Calculate()
        {
            var uix = new UnitInterval(height*uvSize);
            var uiy = new UnitInterval(height*uvSize);
            var hv = transform.up*height;
            aBottom = new List<Vector3> { transform.GetChild(0).localPosition };
            bBottom = new List<Vector3>();
            aTop = new List<Vector3> { aBottom[0] + hv + transform.up * transform.GetChild(0).GetComponent<WallPoint>().heightOffset };
            bTop = new List<Vector3>();

            //Generate general points
            int currentChild = 0;
            while (currentChild < transform.childCount - 1)
            {
                var awp = transform.GetChild(currentChild).GetComponent<WallPoint>();
                var a = awp.transform.localPosition;
                var atop = a + (hv + transform.up * awp.heightOffset);
                var bwp = transform.GetChild(currentChild + 1).GetComponent<WallPoint>();
                var b = bwp.transform.localPosition;
                var btop = b + (hv + transform.up * bwp.heightOffset);
                var isBezier = bwp.isCurveCenter;
                if (isBezier)
                {
                    var cwp = transform.GetChild(currentChild + 2).GetComponent<WallPoint>();
                    var c = cwp.transform.localPosition;
                    var ctop = c + (hv + transform.up * cwp.heightOffset);

                    var bezierPoints = new Vector3[100];
                    for (int i = 0; i < 100; i++)
                        bezierPoints[i] = GetBezierPoint(0.01f*i, a, b, c);
                    var bezierLength = 0f;
                    for (int i = 0; i < 99; i++)
                        bezierLength += Vector3.Distance(bezierPoints[i], bezierPoints[i + 1]);

                    

                    var steps = (int) (bezierLength/smoothDistance);
                    for (int i = 1; i < steps + 1; i++)
                    {
                        aBottom.Add(GetBezierPoint(1f/steps*i, a, b, c));
                        aTop.Add(GetBezierPoint(1f / steps * i, atop, btop, ctop));
                    }


                }
                else
                {
                    aBottom.Add(b);
                    aTop.Add(btop);
                }

                if (isBezier)
                    currentChild++;
                currentChild++;
            }

            var last = aBottom.Count - 1;
            //Generate other points
            bBottom.Add(aBottom[0] + GetBrotherPoint(aBottom[1], aBottom[0]));
            for (int i = 1; i < aBottom.Count-1; i++)
            {
                bBottom.Add(aBottom[i] + GetBrotherPoint(aBottom[i - 1], aBottom[i], aBottom[i + 1]));
            }
            bBottom.Add(aBottom[last] + GetBrotherPoint(aBottom[last], aBottom[last - 1]));

            bTop.Add(aTop[0] + GetBrotherPoint(aTop[1], aTop[0]));
            for (int i = 1; i < aTop.Count - 1; i++)
            {
                bTop.Add(aTop[i] + GetBrotherPoint(aTop[i - 1], aTop[i], aTop[i + 1]));
            }
            bTop.Add(aTop[last] + GetBrotherPoint(aTop[last], aTop[last - 1]));


            //Created meshes - порядок диктуется порядком в развертке - важная деталь
            //в начале
            var startMesh = Quad(bTop[0], bBottom[0], aBottom[0], aTop[0]);
            {
                var uvs = new List<Vector2>();
                var dist = Vector3.Distance(aBottom[0], aTop[0]);
                uvs.Add(new Vector2(uix.Get(), uiy.Get(dist)));
                uvs.Add(new Vector2(uix.Get(), 0));
                uix.Plus(Vector3.Distance(aBottom[0], bBottom[0]));
                uvs.Add(new Vector2(uix.Get(), 0));
                uvs.Add(new Vector2(uix.Get(), uiy.Get(dist)));
                startMesh.uv = uvs.ToArray();
            }
            //слева
            var aMesh = BendPlane(aBottom.ToArray(), aTop.ToArray(), false);
            {
                var uvs = new List<Vector2>();
                for (int i = 0; i < aBottom.Count; i++)
                {
                    var dist = Vector3.Distance(aBottom[i], aTop[i]);
                    if (i > 0)
                        uix.Plus(Vector3.Distance(aBottom[i - 1], aBottom[i]));
                    uvs.Add(new Vector2(uix.Get(), 0));
                    uvs.Add(new Vector2(uix.Get(), uiy.Get(dist)));
                }
                aMesh.uv = uvs.ToArray();
            }
            //в конце
            var endMesh = Quad(aTop[last], aBottom[last], bBottom[last], bTop[last]);
            {
                var uvs = new List<Vector2>();
                var dist = Vector3.Distance(aBottom[last], aTop[last]);
                uvs.Add(new Vector2(uix.Get(), uiy.Get(dist)));
                uvs.Add(new Vector2(uix.Get(), 0));
                uix.Plus(Vector3.Distance(aBottom[last], bBottom[last]));
                uvs.Add(new Vector2(uix.Get(), 0));
                uvs.Add(new Vector2(uix.Get(), uiy.Get(dist)));
                endMesh.uv = uvs.ToArray();
            }
            //справа
            var bMesh = BendPlane(bBottom.ToArray(), bTop.ToArray(), true);
            {
                var uvs = new List<Vector2>();
                for (int i = bBottom.Count-1; i >= 0; i--)
                {
                    var dist = Vector3.Distance(bBottom[i], bTop[i]);
                    if (i < bBottom.Count - 1)
                        uix.Minus(Vector3.Distance(bBottom[i], bBottom[i + 1]));
                    uvs.Insert(0, new Vector2(uix.Get(), uiy.Get(dist)));
                    uvs.Insert(0, new Vector2(uix.Get(), 0));
                    
                }
                bMesh.uv = uvs.ToArray();
            }
            
            
            
            //сверху
            var topMesh = BendPlane(aTop.ToArray(), bTop.ToArray(), false);
            {
                var uvs = new List<Vector2>();
                for (int i = 0; i < aTop.Count; i++)
                {
                    var dist = Vector3.Distance(aTop[i], bTop[i]);
                    if (i > 0)
                        uix.Plus(Vector3.Distance(aTop[i - 1], aTop[i]));
                    uvs.Add(new Vector2(uix.Get(), 0));
                    uvs.Add(new Vector2(uix.Get(), uiy.Get(dist)));
                }
                topMesh.uv = uvs.ToArray();
            }

            var combineMesh = CombineMerge(true, aMesh, bMesh, startMesh, endMesh);
            combineMesh = CombineSub(true, combineMesh, topMesh);
            combineMesh.RecalculateNormals();
            var f = GetComponent<MeshFilter>();
            if (f.sharedMesh)
                DestroyImmediate(f.sharedMesh);
            f.sharedMesh = combineMesh;
        }

        public Vector3 GetBrotherPoint(Vector3 a, Vector3 b)
        {
            var norm = (new Vector3(a.x, 0, a.z) - new Vector3(b.x, 0, b.z)).normalized;
            return Quaternion.Euler(0, 90, 0) * norm * deep;
        }

        public Vector3 GetBrotherPoint(Vector3 a, Vector3 b, Vector3 c)
        {
            var norm1 = (new Vector3(c.x, 0, c.z) - new Vector3(b.x, 0, b.z)).normalized;
            var norm2 = (new Vector3(b.x, 0, b.z) - new Vector3(a.x, 0, a.z)).normalized;
            return Quaternion.Euler(0, 90, 0) * Vector3.Slerp(norm1, norm2, 0.5f) * deep;
        }

        public Vector3 GetBezierPoint(float t, Vector3 a, Vector3 b, Vector3 c)
        {
            float minusT = 1f - t,
                  aCoef = minusT * minusT,
                  bCoef = 2 * t * minusT,
                  cCoef = t * t;
            return new Vector3(a.x * aCoef + b.x * bCoef + c.x * cCoef,
                               a.y * aCoef + b.y * bCoef + c.y * cCoef,
                               a.z * aCoef + b.z * bCoef + c.z * cCoef);
        }

        public static Mesh CombineMerge(bool destroy, params Mesh[] meshes)
        {
            var mesh = new Mesh();
            var combine = new CombineInstance[meshes.Length];
            for (int i = 0; i < meshes.Length; i++)
                combine[i].mesh = meshes[i];
            mesh.CombineMeshes(combine, true, false);
            if (destroy)
            {
                for (int i = 0; i < meshes.Length; i++)
                    DestroyImmediate(meshes[i]);
            }
            return mesh;
        }

        public static Mesh CombineSub(bool destroy, params Mesh[] meshes)
        {
            var mesh = new Mesh();
            var combine = new CombineInstance[meshes.Length];
            for (int i = 0; i < meshes.Length; i++)
                combine[i].mesh = meshes[i];
            mesh.CombineMeshes(combine, false, false);
            if (destroy)
            {
                for (int i = 0; i < meshes.Length; i++)
                    DestroyImmediate(meshes[i]);
            }
            return mesh;
        }

        public static Mesh BendPlane(Vector3[] bottomPoints, Vector3[] topPoints, bool invertTriangles)
        {
            var ps = new List<Vector3>();

            var trs = new List<int>();

            var uvs = new List<Vector2>();

            for (int i = 0; i < bottomPoints.Length; i++)
            {
                ps.Add(bottomPoints[i]);
                ps.Add(topPoints[i]);
            }
            for (int i = 0; i < bottomPoints.Length - 1; i++)
            {
                trs.Add(i * 2 + 0);
                if (invertTriangles)
                {
                    trs.Add(i * 2 + 1);
                    trs.Add(i * 2 + 2);
                }
                else
                {
                    trs.Add(i * 2 + 2);
                    trs.Add(i * 2 + 1);
                }

                trs.Add(i * 2 + 1);

                if (invertTriangles)
                {
                    trs.Add(i * 2 + 3);
                    trs.Add(i * 2 + 2);
                }
                else
                {
                    trs.Add(i * 2 + 2);
                    trs.Add(i * 2 + 3);
                }
            }
            for (int i = 0; i < bottomPoints.Length; i++)
            {
                uvs.Add(new Vector2(1f / bottomPoints.Length * i, 0));
                uvs.Add(new Vector2(1f / bottomPoints.Length * i, 1));
            }

            var mesh = new Mesh
            {
                vertices = ps.ToArray(),
                triangles = trs.ToArray(),
                uv = uvs.ToArray()
            };
            mesh.RecalculateNormals();
            return mesh;
        }

        public static Mesh Quad(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            var normal = Vector3.Cross(a, c).normalized;
            var mesh = new Mesh
            {
                vertices = new[] { a, b, c, d },
                normals = new[] { normal, normal, normal, normal },
                uv = new[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0) },
                triangles = new[] { 0, 1, 2, 0, 2, 3 }
            };
            return mesh;
        }
    }
}