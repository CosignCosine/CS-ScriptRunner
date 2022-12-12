using ICities;
using System;
using UnityEngine;

namespace CS_ScriptRunner
{
    public class Mod : IUserMod
    {
        public string Name => "Developer Script Wrapper";
        public string Description => "Provides a wrapper to run some developer scripts without ModTools.";

        public int submeshNumber = 0;

        public int axis = 1;

        public string[] axisChoices = new string[]
        {
            "x",
            "y",
            "z"
        };

        public int shaderIndex = 0;
        public string shader => shaderChoices[shaderIndex];

        public string[] shaderChoices = new string[]
        {
            "Custom/Vehicles/Vehicle/Rotors",
            "Custom/Buildings/Building/NoBase",
            "Custom/Buildings/Building/Floating",
            "Custom/Props/Prop/Default",
            "Custom/Props/Prop/Fence",
        };

        public bool deleteLOD = true;

        public void OnSettingsUI(UIHelperBase helper)
        {
            UIHelperBase global = helper.AddGroup("Global Settings");
            global.AddDropdown("Shader", shaderChoices, shaderIndex, (int index) =>
            {
                shaderIndex = index;
            });

            global.AddCheckbox("Also delete LOD", deleteLOD, (bool dlod) =>
            {
                deleteLOD = dlod;
            });

            UIHelperBase mainMeshRotors = helper.AddGroup("Main Mesh");
            mainMeshRotors.AddButton("Set Shader", () =>
            {
                try
                {
                    ShaderMain();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            });

            
            UIHelperBase submeshRotors = helper.AddGroup("Submesh");

            submeshRotors.AddTextfield("Submesh Number", submeshNumber.ToString(), (string num) =>
            {
                if(int.TryParse(num, out submeshNumber))
                {
                    Debug.Log("Submesh number set to " + submeshNumber);
                }
            });

            submeshRotors.AddDropdown("Axis (for Vehicles)", axisChoices, 1, (int axisNumber) =>
            {
                axis = axisNumber;
            });

            submeshRotors.AddButton("Set Shader", () =>
            {
                try
                {
                    ShaderSub();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            });
        }

        private void ShaderMain()
        {
            Shader selectedShader = Shader.Find(shader);

            BuildingInfo buildingAsset = ToolsModifierControl.toolController.m_editPrefabInfo as BuildingInfo;
            VehicleInfo vehicleAsset = ToolsModifierControl.toolController.m_editPrefabInfo as VehicleInfo;
            PropInfo propAsset = ToolsModifierControl.toolController.m_editPrefabInfo as PropInfo;

            if (buildingAsset)
            {
                if (buildingAsset.m_material != null) buildingAsset.m_material.shader = selectedShader;

                if (deleteLOD)
                {
                    buildingAsset.m_lodMesh = null;
                    buildingAsset.m_lodMaterial = null;
                    buildingAsset.m_lodMaterialCombined = null;
                    buildingAsset.m_lodObject = null;
                }
                else if (buildingAsset.m_lodMaterial != null)
                {
                    buildingAsset.m_lodMaterial.shader = selectedShader;
                }
            }else if (vehicleAsset)
            {
                if (vehicleAsset.m_material != null) vehicleAsset.m_material.shader = selectedShader;

                if (deleteLOD)
                {
                    vehicleAsset.m_lodMesh = null;
                    vehicleAsset.m_lodMaterial = null;
                    vehicleAsset.m_lodMaterialCombined = null;
                    vehicleAsset.m_lodObject = null;
                }
                else if (vehicleAsset.m_lodMaterial != null)
                {
                    vehicleAsset.m_lodMaterial.shader = selectedShader;
                }
            }
            else if (propAsset)
            {
                if (propAsset.m_material != null) propAsset.m_material.shader = selectedShader;

                if (deleteLOD)
                {
                    propAsset.m_lodMesh = null;
                    propAsset.m_lodMaterial = null;
                    propAsset.m_lodMaterialCombined = null;
                    propAsset.m_lodObject = null;
                }
                else if (propAsset.m_lodMaterial != null)
                {
                    propAsset.m_lodMaterial.shader = selectedShader;
                }
            }
        }

        private void ShaderSub()
        {
            Shader selectedShader = Shader.Find(shader);
            BuildingInfo buildingAsset = ToolsModifierControl.toolController.m_editPrefabInfo as BuildingInfo;
            VehicleInfo vehicleAsset = ToolsModifierControl.toolController.m_editPrefabInfo as VehicleInfo;

            if (buildingAsset)
            {
                buildingAsset.m_subMeshes[submeshNumber].m_subInfo.m_material.shader = selectedShader;
                buildingAsset.m_subMeshes[submeshNumber].m_subInfo.m_lodMaterial.shader = selectedShader;
                buildingAsset.m_subMeshes[submeshNumber].m_subInfo.m_lodMesh = null;
                buildingAsset.m_subMeshes[submeshNumber].m_subInfo.m_lodMaterial = null;
                buildingAsset.m_subMeshes[submeshNumber].m_subInfo.m_lodMaterialCombined = null;
                buildingAsset.m_subMeshes[submeshNumber].m_subInfo.m_lodObject = null;
            }
            else if (vehicleAsset)
            {
                VehicleInfoBase submesh = vehicleAsset.m_subMeshes[submeshNumber].m_subInfo;
                submesh.m_material.shader = selectedShader;
                Vector3[] meshVertices = submesh.m_mesh.vertices; 
                byte b = 255;

                switch (axis)
                {
                    case 0:
                    case 1:
                        b = 0;
                        break;
                    case 2:
                        submesh.m_UIPriority = 120122;
                        break;
                }

                Color[] colors = new Color[meshVertices.Length];
                for (int i = 0; i < meshVertices.Length; i++) colors[i] = new Color32(128, (byte)(i * 8), b, 255);
                submesh.m_mesh.colors = colors; 

                Vector4[] tyres = new Vector4[meshVertices.Length];
                for (int i = 0; i < meshVertices.Length; i++) tyres[i] = new Vector4(meshVertices[i].x, meshVertices[i].y, meshVertices[i].z, 0);
                submesh.m_generatedInfo.m_tyres = tyres; 

                submesh.m_mesh.RecalculateTangents(); 
                submesh.m_material.name = "rotsm-" + axis + "-" + submeshNumber;
            }
        }
    }
}
