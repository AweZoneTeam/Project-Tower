/* 
 * Created by Alex 'Extravert' Kasaurov
 * From  xgm.guru community
 */
using UnityEngine;

namespace XGM.GURU
{
    public class WallPoint : MonoBehaviour
    {
        public bool isCurveCenter;
        public float heightOffset;
        public void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, 0.25f);
        }
    }
}