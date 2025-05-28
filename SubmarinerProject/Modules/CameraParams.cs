using RoR2;
using UnityEngine;

internal enum SubmarinerCameraParams
{
    DEFAULT,
    EMOTE
}

namespace SubmarinerMod.Modules
{
    internal static class CameraParams
    {
        internal static CharacterCameraParamsData defaultCameraParams;
        internal static CharacterCameraParamsData sniperAimCameraParams;
        internal static CharacterCameraParamsData emoteCameraParams;

        internal static void InitializeParams()
        {
            defaultCameraParams = NewCameraParams("ccpSubmariner", 70f, 1.37f, new Vector3(0f, 0f, -8.1f));
        }

        private static CharacterCameraParamsData NewCameraParams(string name, float pitch, float pivotVerticalOffset, Vector3 standardPosition)
        {
            return NewCameraParams(name, pitch, pivotVerticalOffset, standardPosition, 0.1f);
        }

        private static CharacterCameraParamsData NewCameraParams(string name, float pitch, float pivotVerticalOffset, Vector3 idealPosition, float wallCushion)
        {
            CharacterCameraParamsData newParams = new CharacterCameraParamsData();

            newParams.maxPitch = pitch;
            newParams.minPitch = -pitch;
            newParams.pivotVerticalOffset = pivotVerticalOffset;
            newParams.idealLocalCameraPos = idealPosition;
            newParams.wallCushion = wallCushion;

            return newParams;
        }

        internal static CameraTargetParams.CameraParamsOverrideHandle OverrideCameraParams(CameraTargetParams camParams, SubmarinerCameraParams camera, float transitionDuration = 0.5f)
        {
            CharacterCameraParamsData paramsData = GetNewParams(camera);

            CameraTargetParams.CameraParamsOverrideRequest request = new CameraTargetParams.CameraParamsOverrideRequest
            {
                cameraParamsData = paramsData,
                priority = 0,
            };

            return camParams.AddParamsOverride(request, transitionDuration);
        }

        internal static CharacterCameraParams CreateCameraParamsWithData(SubmarinerCameraParams camera)
        {

            CharacterCameraParams newCameraParams = ScriptableObject.CreateInstance<CharacterCameraParams>();

            newCameraParams.name = camera.ToString().ToLower() + "Params";

            newCameraParams.data = GetNewParams(camera);

            return newCameraParams;
        }

        internal static CharacterCameraParamsData GetNewParams(SubmarinerCameraParams camera)
        {
            CharacterCameraParamsData paramsData = defaultCameraParams;

            switch (camera)
            {

                default:
                case SubmarinerCameraParams.DEFAULT:
                    paramsData = defaultCameraParams;
                    break;
                case SubmarinerCameraParams.EMOTE:
                    paramsData = emoteCameraParams;
                    break;
            }

            return paramsData;
        }
    }
}