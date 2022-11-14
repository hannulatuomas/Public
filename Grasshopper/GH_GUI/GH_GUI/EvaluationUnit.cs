using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;

namespace GH_GUI
{
    public class EvaluationUnit
    {
        private GH_SwitcherComponent compontent;

        private string name;

        private string displayName;

        private string description;

        private List<ExtendedPlug> inputs;

        private List<ExtendedPlug> outputs;

        private bool active;

        private Bitmap icon;

        private bool keepLinks;

        private EvaluationUnitContext cxt;

        public GH_SwitcherComponent Component
        {
            get
            {
                return this.compontent;
            }
            set
            {
                this.compontent = value;
            }
        }

        public List<ExtendedPlug> Inputs
        {
            get
            {
                return this.inputs;
            }
        }

        public List<ExtendedPlug> Outputs
        {
            get
            {
                return this.outputs;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        public string DisplayName
        {
            get
            {
                return this.displayName;
            }
            set
            {
                this.displayName = value;
            }
        }

        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
            }
        }

        public bool Active
        {
            get
            {
                return this.active;
            }
            set
            {
                this.active = value;
            }
        }

        public bool KeepLinks
        {
            get
            {
                return this.keepLinks;
            }
            set
            {
                this.keepLinks = value;
            }
        }

        public Bitmap Icon
        {
            get
            {
                return this.icon;
            }
            set
            {
                this.icon = value;
            }
        }

        public EvaluationUnitContext Context
        {
            get
            {
                return this.cxt;
            }
        }

        public EvaluationUnit(string name, string displayName, string description, Bitmap icon = null)
        {
            this.name = name;
            this.displayName = displayName;
            this.description = description;
            this.inputs = new List<ExtendedPlug>();
            this.outputs = new List<ExtendedPlug>();
            this.keepLinks = false;
            this.icon = icon;
            this.cxt = new EvaluationUnitContext(this);
        }

        private static Type GetGenericType(Type generic, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                Type right = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == right)
                {
                    return toCheck;
                }
                toCheck = toCheck.BaseType;
            }
            return null;
        }

        public void RegisterInputParam(IGH_Param param, string name, string nickName, string description, GH_ParamAccess access, IGH_Goo defaultValue)
        {
            param.Name = name;
            param.NickName = nickName;
            param.Description = description;
            param.Access = access;
            try
            {
                if (defaultValue != null && typeof(IGH_Goo).IsAssignableFrom(param.Type))
                {
                    Type genericType = EvaluationUnit.GetGenericType(typeof(GH_PersistentParam<>), param.GetType());
                    if (genericType != null)
                    {
                        Type[] genericArguments = genericType.GetGenericArguments();
                        if (genericArguments.Length > 0)
                        {
                            Type type = genericArguments[0];
                            MethodInfo method = genericType.GetMethod("SetPersistentData", BindingFlags.Instance | BindingFlags.Public, null, new Type[1]
                            {
                                genericArguments[0]
                            }, null);
                            method.Invoke(param, new object[1]
                            {
                                defaultValue
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            ExtendedPlug extendedPlug = new ExtendedPlug(param);
            extendedPlug.Unit = this;
            this.inputs.Add(extendedPlug);
        }

        public void RegisterInputParam(IGH_Param param, string name, string nickName, string description, GH_ParamAccess access)
        {
            this.RegisterInputParam(param, name, nickName, description, access, null);
        }

        public bool UnregisterInputParam(string name)
        {
            for (int i = 0; i < this.inputs.Count; i++)
            {
                ExtendedPlug input = this.inputs[i];
                if (input.Unit.Name == name)
                {
                    this.inputs.RemoveAt(i);
                    this.compontent.Params.UnregisterInputParameter(input.Parameter);
                    return true;
                }
            }
            return false;
        }

        public void RegisterOutputParam(IGH_Param param, string name, string nickName, string description)
        {
            param.Name = name;
            param.NickName = nickName;
            param.Description = description;
            ExtendedPlug extendedPlug = new ExtendedPlug(param);
            extendedPlug.Unit = this;
            this.outputs.Add(extendedPlug);
        }
        public bool UnregisterOutputParam(string name)
        {
            for (int i = 0; i < this.outputs.Count; i++)
            {
                ExtendedPlug output = this.outputs[i];
                if (output.Unit.Name == name)
                {
                    this.outputs.RemoveAt(i);
                    this.compontent.Params.UnregisterOutputParameter(output.Parameter);
                    return true;
                }
            }
            return false;
        }

        public void NewParameterIds()
        {
            foreach (ExtendedPlug current in this.inputs)
            {
                current.Parameter.NewInstanceGuid();
            }
            foreach (ExtendedPlug current2 in this.outputs)
            {
                current2.Parameter.NewInstanceGuid();
            }
        }

        public void AddMenu(GH_ExtendableMenu menu)
        {
            this.Context.Collection.AddMenu(menu);
        }

        public bool Write(GH_IWriter writer)
        {
            writer.SetString("name", this.Name);
            GH_IWriter gH_IWriter = writer.CreateChunk("params");
            GH_IWriter gH_IWriter2 = gH_IWriter.CreateChunk("input");
            gH_IWriter2.SetInt32("index", 0);
            gH_IWriter2.SetInt32("count", this.Inputs.Count);
            for (int i = 0; i < this.inputs.Count; i++)
            {
                if (this.inputs[i].Parameter.Attributes != null)
                {
                    GH_IWriter writer2 = gH_IWriter2.CreateChunk("p", i);
                    this.inputs[i].Parameter.Write(writer2);
                }
            }
            GH_IWriter gH_IWriter3 = gH_IWriter.CreateChunk("output");
            gH_IWriter3.SetInt32("index", 0);
            gH_IWriter3.SetInt32("count", this.Outputs.Count);
            for (int j = 0; j < this.outputs.Count; j++)
            {
                if (this.outputs[j].Parameter.Attributes != null)
                {
                    GH_IWriter writer3 = gH_IWriter3.CreateChunk("p", j);
                    this.outputs[j].Parameter.Write(writer3);
                }
            }
            GH_IWriter writer4 = writer.CreateChunk("context");
            return this.cxt.Collection.Write(writer4);
        }

        public bool Read(GH_IReader reader)
        {
            if (reader.ChunkExists("params"))
            {
                GH_IReader gH_IReader = reader.FindChunk("params");
                if (gH_IReader.ChunkExists("input") && this.inputs != null)
                {
                    GH_IReader gH_IReader2 = gH_IReader.FindChunk("input");
                    int num = -1;
                    if (gH_IReader2.TryGetInt32("count", ref num) && this.inputs.Count == num)
                    {
                        for (int i = 0; i < num; i++)
                        {
                            if (gH_IReader2.ChunkExists("p", i))
                            {
                                this.inputs[i].Parameter.Read(gH_IReader2.FindChunk("p", i));
                            }
                            else if (gH_IReader2.ChunkExists("param", i))
                            {
                                this.inputs[i].Parameter.Read(gH_IReader2.FindChunk("param", i));
                            }
                        }
                    }
                }
                if (gH_IReader.ChunkExists("output") && this.outputs != null)
                {
                    GH_IReader gH_IReader3 = gH_IReader.FindChunk("output");
                    int num2 = -1;
                    if (gH_IReader3.TryGetInt32("count", ref num2) && this.outputs.Count == num2)
                    {
                        for (int j = 0; j < num2; j++)
                        {
                            if (gH_IReader3.ChunkExists("p", j))
                            {
                                this.outputs[j].Parameter.Read(gH_IReader3.FindChunk("p", j));
                            }
                            else if (gH_IReader3.ChunkExists("param", j))
                            {
                                this.outputs[j].Parameter.Read(gH_IReader3.FindChunk("param", j));
                            }
                        }
                    }
                }
            }
            try
            {
                GH_IReader gH_IReader4 = reader.FindChunk("context");
                if (gH_IReader4 != null)
                {
                    this.cxt.Collection.Read(gH_IReader4);
                }
            }
            catch (Exception ex)
            {
                Rhino.RhinoApp.WriteLine("Component error:" + ex.Message + "\n" + ex.StackTrace);
            }
            return true;
        }
    }
}
