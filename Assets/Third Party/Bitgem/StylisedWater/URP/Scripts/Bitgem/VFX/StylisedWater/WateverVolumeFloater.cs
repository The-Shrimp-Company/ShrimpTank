#region Using statements

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Bitgem.VFX.StylisedWater
{
    public class WateverVolumeFloater : MonoBehaviour
    {
        #region Public fields

        public WaterVolumeHelper WaterVolumeHelper = null;
        public float heightOffset = 0;
        private Vector3 startingPosition;

        #endregion

        #region MonoBehaviour events


        private void Awake()
        {
            startingPosition = transform.position;
        }


        void Update()
        {
            var instance = WaterVolumeHelper ? WaterVolumeHelper : WaterVolumeHelper.Instance;
            if (!instance)
            {
                return;
            }

            if (DecorateTankController.Instance.decorating)
            {
                transform.position = startingPosition;
                return;
            }

            transform.position = new Vector3(transform.position.x, (instance.GetHeight(transform.position) + heightOffset) ?? transform.position.y, transform.position.z);
        }

        #endregion
    }
}