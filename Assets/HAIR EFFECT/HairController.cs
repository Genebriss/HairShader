using System.Collections.Generic;
using UnityEngine;

public class HairController : MonoBehaviour
{
    public Transform headBone;
    public Vector3 collisionSphereOffset = new Vector3();
    public float collisionSphereRadius = 0.5f;
    public int historyLength = 100;

    public List<Quaternion> quaternionHistory;
    //public List<Vector3> positionHistory; //можно аналогично добавить разницу позишенов, но сейчас посчитал излишним

    void LateUpdate()
    {
        if (headBone == null)
            return;
        
        //сохранение 100 последних фреймов истории ротейшена головы
        quaternionHistory.Add(headBone.rotation);
        if (quaternionHistory.Count>=historyLength)
            quaternionHistory.RemoveAt(0);

        //разница ротейшеном между старым и текущим положением головы
        Quaternion deltaQuaternion = headBone.rotation * Quaternion.Inverse(quaternionHistory[0]);
        float angle = 0.0f;
        Vector3 axis = Vector3.zero;
        deltaQuaternion.ToAngleAxis(out angle, out axis);

        //Ооправка в шейдер. Предполагается что персонаж один, поэтому редактировать конкретный материал не вижу смысла
        Shader.SetGlobalVector("_CurrentHeadPosition", headBone.position);  //пивот для поворота вертексов вокруг него
        Shader.SetGlobalVector("_rotateAxis", axis);
        Shader.SetGlobalFloat("_rotateAngle", angle);

        Shader.SetGlobalVector("_CollisionSphereWorldspacePosition", headBone.TransformPoint(collisionSphereOffset));
        Shader.SetGlobalFloat("_CollisionSphereRadius", collisionSphereRadius);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(headBone.TransformPoint(collisionSphereOffset), collisionSphereRadius);
    }
#endif
}
