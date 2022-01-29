using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNDL_Explorer.Viewer
{
    class OBJImporter
    {
        public struct Face
        {
            public int f1;
            public int f2;
            public int f3;
        };

        List<Vector3> faceVerts = new List<Vector3>();
        List<Face> faceList = new List<Face>();
        List<Vector3> faceNormals = new List<Vector3>();
        List<Vector2> uvList = new List<Vector2>();

        public OBJImporter(string filePath)
        {
            //var file = File.ReadAllText(filePath);

            foreach (string line in File.ReadLines(filePath))
            {
                if (line.StartsWith("vt"))
                {
                    var lineData = line.Split(' ');

                    uvList.Add(new Vector2(
                        Convert.ToSingle(lineData[1]),
                        Convert.ToSingle(lineData[2])
                        ));
                    continue;
                }

                if (line.StartsWith("vn"))
                {
                    var lineData = line.Split(' ');

                    faceNormals.Add(new Vector3(
                        Convert.ToSingle(lineData[1]),
                        Convert.ToSingle(lineData[2]),
                        Convert.ToSingle(lineData[3])
                        ));
                    continue;
                }

                if (line.StartsWith("v"))
                {
                    var lineData = line.Split(' ');

                    faceVerts.Add(new Vector3(
                        Convert.ToSingle(lineData[1]),
                        Convert.ToSingle(lineData[2]),
                        Convert.ToSingle(lineData[3])
                        ));
                    continue;
                }

                if (line.StartsWith("f"))
                {
                    var lineData = line.Split(' ');

                    List<string> tempStr = new List<string>(lineData);

                    tempStr.RemoveAt(0);
                    //tempStr.RemoveAt(tempStr.Count - 1);

                    List<int> fList = new List<int>();
                    for (int i = 0; i < tempStr.Count; i++)
                    {
                        //int t = tempStr[0].IndexOf("/");

                        fList.Add(Convert.ToInt32(tempStr[i].Substring(0, tempStr[i].IndexOf("/"))));
                    }

                    Face face = new Face();
                    face.f1 = fList[0];
                    face.f2 = fList[1];
                    face.f3 = fList[2];

                    faceList.Add(face);
                    //tempStr.RemoveAt(0);
                    continue;
                }
            }
        }

        public float[] GetVBO()
        {
            List<float> vboData = new List<float>();
            
            
            foreach (Face item in faceList)
            {
                vboData.Add(faceVerts[item.f1-1][0]);
                vboData.Add(faceVerts[item.f1-1][1]);
                vboData.Add(faceVerts[item.f1-1][2]);

                vboData.Add(faceNormals[item.f1-1][0]);
                vboData.Add(faceNormals[item.f1-1][1]);
                vboData.Add(faceNormals[item.f1-1][2]);

                vboData.Add(0.0f);
                vboData.Add(1.0f);

                vboData.Add(faceVerts[item.f3-1][0]);
                vboData.Add(faceVerts[item.f3-1][1]);
                vboData.Add(faceVerts[item.f3-1][2]);

                vboData.Add(faceNormals[item.f3-1][0]);
                vboData.Add(faceNormals[item.f3-1][1]);
                vboData.Add(faceNormals[item.f3-1][2]);

                vboData.Add(0.0f);
                vboData.Add(1.0f);

                

                vboData.Add(faceVerts[item.f2][0]);
                vboData.Add(faceVerts[item.f2][1]);
                vboData.Add(faceVerts[item.f2][2]);

                vboData.Add(faceNormals[item.f2][0]);
                vboData.Add(faceNormals[item.f2][1]);
                vboData.Add(faceNormals[item.f2][2]);

                vboData.Add(0.0f);
                vboData.Add(1.0f);
            }

            return vboData.ToArray();
        }

        public uint[] GetEBO()
        {
            List<uint> list = new List<uint>();

            foreach (var item in faceList)
            {
                list.Add((uint)item.f1-1);
                list.Add((uint)item.f2-1);
                list.Add((uint)item.f3-1);
            }
            return list.ToArray();
        }

    }
}
