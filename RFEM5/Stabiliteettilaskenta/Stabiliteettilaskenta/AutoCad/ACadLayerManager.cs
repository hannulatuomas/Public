using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoCAD;

namespace Stabiliteettilaskenta.AutoCad
{
    public class ACadLayerManager
    {
        public Dictionary<string, bool[]> layerStates;
        public Dictionary<string, bool[]> GetLayerStates(AcadDocument cadDocument)
        {
            Dictionary<string, bool[]> layerNames = new Dictionary<string, bool[]>();

            AcadLayers layers = cadDocument.Layers;
            bool[] states;

            for (int i = 0; i < layers.Count; i++)
            {
                AcadLayer layer = layers.Item(i);
                string name = layer.Name;
                bool layerOn = layer.LayerOn;
                bool freezed = layer.Freeze;
                bool locked = layer.Lock;
                bool plottable = layer.Plottable;
                bool used = layer.Used;
                states = new bool[] { layerOn, freezed, locked, plottable, used };
                layerNames.Add(name, states);
            }
            layerStates = layerNames;
            return layerNames;
        }

        public void SetLayerStates(AcadDocument cadDocument, Dictionary<string, bool[]> layerStates)
        {
            AcadLayers layers = cadDocument.Layers;
            AcadLayer activeLayer = cadDocument.ActiveLayer;

            for (int i = 0; i < layers.Count; i++)
            {
                AcadLayer layer = layers.Item(i);
                string name = layer.Name;
                bool[] states;

                if (layer == activeLayer)
                {
                    continue;
                }
                if (layerStates.TryGetValue(name, out states))
                {
                    if (states.Length == 5)
                    {
                        layer.LayerOn = states[0];
                        layer.Freeze = states[1];
                        layer.Lock = states[2];
                        layer.Plottable = states[3];
                    }
                }
            }
        }

        public void EmptyLayer(AcadDocument cadDocument, string layername)
        {
            AcadDatabase database = cadDocument.Database;
            var data = database.ModelSpace.GetEnumerator();

            while (data.MoveNext())
            {
                AcadEntity item = (AcadEntity)data.Current;

                if (item.Layer == layername)
                {
                    item.Delete();
                }
            }
        }

        public void DeleteLayer(AcadDocument cadDocument, string layername)
        {
            AcadLayers layers = cadDocument.Layers;
            AcadLayer activeLayer = cadDocument.ActiveLayer;
            AcadLayer layer;

            if (activeLayer.Name == layername)
            {
                return;
            }

            for (int i = 0; i < layers.Count; i++)
            {
                layer = layers.Item(i);

                if (layer.Name == layername)
                {
                    if (layer.Used)
                    {
                        EmptyLayer(cadDocument, layername);
                    }

                    layer.Delete();
                    return;
                }
            }
        }
    }
}
