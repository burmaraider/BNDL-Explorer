using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNDL_Explorer.Classes
{
    public struct Header
    {
        public char[] magic;
        public int version;
        public byte[] unk1;
        public uint dat_size;
        public byte[] unk2;
        public byte[] unk3;
        public uint verts_size;
        public uint tris_size;
    };

    public class Mesh
    {

        MemoryStream _stream;

        Header _header;

        float[] vertexData;
        ushort[] trisData;


        public Mesh() { }

        public void LoadMesh(byte[] meshFile)
        {
            _stream = new MemoryStream(meshFile);
            BinaryReader br = new BinaryReader(_stream);

            //read header
            _header.magic = br.ReadChars(4);
            _header.version = br.ReadInt32();
            _header.unk1 = br.ReadBytes(16);
            _header.dat_size = br.ReadUInt32();
            _header.unk2 = br.ReadBytes(2 * sizeof(int));
            int[] a = new int[2];
            Buffer.BlockCopy(_header.unk2, 0, a, 0, _header.unk2.Length);
            _header.unk3 = br.ReadBytes(a[0]/4 * sizeof(int));
            _header.verts_size = br.ReadUInt32();
            _header.tris_size = br.ReadUInt32();

            //vertex data
            vertexData = new float[_header.verts_size / 4];
            byte[] vertRawData = br.ReadBytes((int)_header.verts_size);
            Buffer.BlockCopy(vertRawData, 0, vertexData, 0, vertRawData.Length / 4);

            //tris data
            trisData = new ushort[_header.tris_size / 2];
            byte[] trisRawData = br.ReadBytes((int)_header.tris_size);
            Buffer.BlockCopy(trisRawData, 0, trisData, 0, trisRawData.Length / 2);


        }

        public void ReadHeader()
        {

        }

        public void BaryTest()
        {
            System.Reflection.Assembly test = System.Reflection.Assembly.GetExecutingAssembly();

            BinaryReader br = new BinaryReader(test.GetManifestResourceStream("BNDL_Explorer.Viewer.data.bin"));

            for (int i = 0; i < 36; i++)
            {
                Vector3 p1 = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                Vector3 b1 = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                br.BaseStream.Position += 4;

                Vector3 p2 = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                Vector3 b2 = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                br.BaseStream.Position += 4;

                Vector3 p3 = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                Vector3 b3 = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                br.BaseStream.Position += 4;

                //br.ReadSingle();
                Barycentric barycentric = new Barycentric(p1, p2, p3, b1);


            }
        }

    }
}
