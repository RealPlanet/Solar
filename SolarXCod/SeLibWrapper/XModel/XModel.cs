using SELib;
using SolarXUtility;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;

namespace SolarXCod.SeLibWrapper.XModel
{
    /// <summary>
    /// Handles writing SEModel files into xmodel_bin/export files.
    /// Original source https://github.com/dtzxporter/CODTools/ and
    /// adapted to work in this project.
    /// </summary>
    internal class XModel
    {
        public string Name { get; set; }
        public List<string> Comments { get; set; } = new List<string>();

        private readonly SEModel _model;

        public XModel(SEModel model)
        {
            _model = model;
        }

        public uint VertexCount()
        {
            uint Result = 0;

            foreach (SEModelMesh Mesh in _model.Meshes)
                Result += Mesh.VertexCount;

            return Result;
        }

        public uint FaceCount()
        {
            uint Result = 0;

            foreach (SEModelMesh Mesh in _model.Meshes)
                Result += Mesh.FaceCount;

            return Result;
        }

        public void ValidateXModel()
        {
            // Perform basic validation, adding root bone, having a material
            if (_model.Bones.Count == 0)
            {
                SEModelBone origin = new SEModelBone
                {
                    BoneName = "tag_origin",
                    GlobalPosition = new SELib.Utilities.Vector3(0, 0, 0)
                };
                _model.Bones.Add(origin);
            }

            if (_model.Materials.Count == 0)
            {
                SEModelMaterial material = new SEModelMaterial
                {
                    Name = "default"
                };
                _model.Materials.Add(material);
            }

        }

        /// <summary>
        /// Generates the file structure for a XMODEL_EXPORT model from a SEMODEL
        /// </summary>
        /// <param name="Stream"> The stream where this file structure will be written</param>
        public void WriteExport(Stream Stream)
        {
            StreamWriter Writer = new StreamWriter(Stream);
            ValidateXModel();

            #region Metadata
            foreach (string Comment in this.Comments)
                Writer.WriteLine("// {0}", Comment);
            #endregion

            // Required format version
            Writer.WriteLine("MODEL");
            Writer.WriteLine("VERSION {0}", 6);

            // Bones
            // Cosmetic bones are not set by this export as it has no way to know what they are
            Writer.WriteLine("NUMBONES {0}", _model.Bones.Count);

            // Parent list
            for (int i = 0; i < _model.Bones.Count; i++)
            {
                Writer.WriteLine("BONE {0} {1} \"{2}\"", i, _model.Bones[i].BoneParent, _model.Bones[i].BoneName);
            }

            // Bone information
            for (int i = 0; i < _model.Bones.Count; i++)
            {
                // BIN/EXPORTS ALWAYS USE GLOBAL ROTATION AND POSITIONING
                Writer.WriteLine("BONE {0}", i);
                Writer.WriteLine("OFFSET {0} {1} {2}", MathEx.RoundToNearestFloat(_model.Bones[i].GlobalPosition.X / MathEx.INCH).ToString("0.00000", CultureInfo.InvariantCulture),
                                                        MathEx.RoundToNearestFloat(_model.Bones[i].GlobalPosition.Y / MathEx.INCH).ToString("0.00000", CultureInfo.InvariantCulture),
                                                        MathEx.RoundToNearestFloat(_model.Bones[i].GlobalPosition.Z / MathEx.INCH).ToString("0.00000", CultureInfo.InvariantCulture));

                Writer.WriteLine("SCALE {0} {1} {2}", MathEx.RoundToNearestFloat(_model.Bones[i].Scale.X).ToString("0.00000", CultureInfo.InvariantCulture),
                                                        MathEx.RoundToNearestFloat(_model.Bones[i].Scale.Y).ToString("0.00000", CultureInfo.InvariantCulture),
                                                        MathEx.RoundToNearestFloat(_model.Bones[i].Scale.Z).ToString("0.00000", CultureInfo.InvariantCulture));

                Quaternion convertedQuat = new System.Numerics.Quaternion((float)_model.Bones[i].GlobalRotation.X,
                                                                    (float)_model.Bones[i].GlobalRotation.Y,
                                                                    (float)_model.Bones[i].GlobalRotation.Z,
                                                                    (float)_model.Bones[i].GlobalRotation.W);

                Matrix4x4 RotationMatrix = Matrix4x4.CreateFromQuaternion(convertedQuat);

                Writer.WriteLine("X {0} {1} {2}", RotationMatrix.M11.ToString("0.00000", CultureInfo.InvariantCulture),
                                                    RotationMatrix.M12.ToString("0.00000", CultureInfo.InvariantCulture),
                                                    RotationMatrix.M13.ToString("0.00000", CultureInfo.InvariantCulture));
                Writer.WriteLine("Y {0} {1} {2}", RotationMatrix.M21.ToString("0.00000", CultureInfo.InvariantCulture),
                                                    RotationMatrix.M22.ToString("0.00000", CultureInfo.InvariantCulture),
                                                    RotationMatrix.M23.ToString("0.00000", CultureInfo.InvariantCulture));
                Writer.WriteLine("Z {0} {1} {2}", RotationMatrix.M31.ToString("0.00000", CultureInfo.InvariantCulture),
                                                    RotationMatrix.M32.ToString("0.00000", CultureInfo.InvariantCulture),
                                                    RotationMatrix.M33.ToString("0.00000", CultureInfo.InvariantCulture));
            }

            // Mesh vertex buffer
            uint VertexBufferSize = this.VertexCount();
            uint FaceBufferSize = this.FaceCount();

            // Global offsets
            int VertexIndex = 0, FaceIndex = 0, MeshIndex = 0;

            // Write count
            Writer.WriteLine("{0} {1}", (VertexBufferSize > ushort.MaxValue) ? "NUMVERTS32" : "NUMVERTS", VertexBufferSize);
            // Iterate and output
            foreach (SEModelMesh Mesh in _model.Meshes)
            {
                foreach (SEModelVertex Vertex in Mesh.Verticies)
                {
                    // Output position, weight data
                    Writer.WriteLine("{0} {1}", (VertexIndex > ushort.MaxValue) ? "VERT32" : "VERT", VertexIndex++);
                    //Writer.WriteLine("OFFSET {0}, {1}, {2}", Vertex.Position.X, Vertex.Position.Y, Vertex.Position.Z);
                    Writer.WriteLine("OFFSET {0} {1} {2}", (Vertex.Position.X / MathEx.INCH).ToString("0.00000", CultureInfo.InvariantCulture),
                                                            (Vertex.Position.Y / MathEx.INCH).ToString("0.00000", CultureInfo.InvariantCulture),
                                                            (Vertex.Position.Z / MathEx.INCH).ToString("0.00000", CultureInfo.InvariantCulture));

                    // Weights
                    Writer.WriteLine("BONES {0}", Vertex.Weights.Count);
                    foreach (SEModelWeight Weight in Vertex.Weights)
                        Writer.WriteLine("BONE {0} {1}", Weight.BoneIndex, Weight.BoneWeight.ToString("0.00000", CultureInfo.InvariantCulture));
                }
            }

            //if (_model.Meshes.Count > 0)
            //    Writer.WriteLine();

            // Write count
            Writer.WriteLine("NUMFACES {0}", FaceBufferSize);
            // Iterate and output
            foreach (SEModelMesh Mesh in _model.Meshes)
            {
                List<SEModelVertex> MeshVerticies = Mesh.Verticies;
                for (int faceIndex = 0; faceIndex < Mesh.Faces.Count; faceIndex++)
                {
                    SEModelFace Face = Mesh.Faces[faceIndex];
                    int[] Indices = { (int)Face.FaceIndex1, (int)Face.FaceIndex2, (int)Face.FaceIndex3 };
                    SEModelVertex[] Verticies = { Mesh.Verticies[Indices[0]], Mesh.Verticies[Indices[1]], Mesh.Verticies[Indices[2]] };

                    // Triangle data
                    int FaceMaterialIndex = Mesh.MaterialReferenceIndicies[0]; // Index is taken from the first UV set
                    Writer.WriteLine("{0} {1} {2} 0 0", (MeshIndex > byte.MaxValue || FaceMaterialIndex > byte.MaxValue) ? "TRI16" : "TRI", MeshIndex, FaceMaterialIndex);

                    // Output triangle vert data
                    for (int i = 0; i < 3; i++)
                    {
                        Writer.WriteLine("{0} {1}", (Indices[i] + FaceIndex > ushort.MaxValue) ? "VERT32" : "VERT", Indices[i] + FaceIndex);
                        Writer.WriteLine("NORMAL {0} {1} {2}", Verticies[i].VertexNormal.X.ToString("0.00000"),
                                                                    Verticies[i].VertexNormal.Y.ToString("0.00000"),
                                                                    Verticies[i].VertexNormal.Z.ToString("0.00000"));
                        // Colors are normalized
                        Writer.WriteLine("COLOR {0} {1} {2} {3}", (Verticies[i].VertexColor.R / 255f).ToString("0.00000"),
                                                                    (Verticies[i].VertexColor.G / 255f).ToString("0.00000"),
                                                                    (Verticies[i].VertexColor.B / 255f).ToString("0.00000"),
                                                                    (Verticies[i].VertexColor.A / 255f).ToString("0.00000"));
                        // Always one UV set
                        Writer.WriteLine("UV 1 {0} {1}", Verticies[i].UVSets[0].X.ToString("0.00000"),
                                                                    Verticies[i].UVSets[0].Y.ToString("0.00000"));
                    }
                }

                // Advance
                FaceIndex += Mesh.Verticies.Count;
                MeshIndex++;
            }

            // Write count
            Writer.WriteLine("NUMOBJECTS {0}", _model.Meshes.Count);
            // Iterate and output
            MeshIndex = 0;
            foreach (SEModelMesh Mesh in _model.Meshes)
            {
                Writer.WriteLine("OBJECT {0} \"SolarXCod_Object_{1}\"", MeshIndex, MeshIndex++);
            }

            // Write count
            Writer.WriteLine("NUMMATERIALS {0}", _model.Materials.Count);
            // Iterate and output
            int MatIndex = 0;
            foreach (SEModelMaterial Mat in _model.Materials)
            {
                object colorData = Mat.MaterialData.GetType().GetProperty("DiffuseMap").GetValue(Mat.MaterialData);
                Writer.WriteLine("MATERIAL {0} \"{1}\" \"Lambert\" \"{2}\"", MatIndex++, Mat.Name, colorData);
                Writer.WriteLine("COLOR 0.000000 0.000000 0.000000 1.000000\nTRANSPARENCY 0.000000 0.000000 0.000000 1.000000\nAMBIENTCOLOR 0.000000 0.000000 0.000000 1.000000\nINCANDESCENCE 0.000000 0.000000 0.000000 1.000000\nCOEFFS 0.800000 0.000000\nGLOW 0.000000 0\nREFRACTIVE 6 1.000000\nSPECULARCOLOR -1.000000 -1.000000 -1.000000 1.000000\nREFLECTIVECOLOR -1.000000 -1.000000 -1.000000 1.000000\nREFLECTIVE -1 -1.000000\nBLINN -1.000000 -1.000000\nPHONG -1.000000");
            }

            // Flush writer
            Writer.Flush();
        }

        public void WriteBin(string filePath)
        {
            // Write to a memory file, then ask the XAssetFile to convert it
            using (MemoryStream Memory = new MemoryStream())
            {
                // Create
                this.WriteExport(Memory);

                // Reset
                Memory.Position = 0;

                // Write it
                using (XAssetFile XAsset = new XAssetFile(Memory, InFormat.Export))
                    XAsset.WriteBin(filePath);
            }
        }
    }
}
