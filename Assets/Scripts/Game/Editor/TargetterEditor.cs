using UnityEditor;
using UnityEngine;

namespace TowerDefense.Targetting.Editor
{
    /// <summary>
    /// The editor for configuring targetter
    /// </summary>
    [CustomEditor(typeof(Targetter)), CanEditMultipleObjects]
    public class TargetterEditor : UnityEditor.Editor
    {
        /// <summary>
        /// The targetter to edit
        /// </summary>
        Targetter m_Targetter;

        /// <summary>
        /// The radius of the collider
        /// </summary>
        float m_ColliderRadius;

        /// <summary>
        /// The attached collider
        /// </summary>
        Collider m_AttachedCollider;

        /// <summary>
        /// The serialized property representing <see cref="m_AttachedCollider"/>
        /// </summary>
        SerializedProperty m_SerializedAttachedCollider;

        /// <summary>
        /// draws the default inspector 
        /// and then draws configuration for colliders
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            AttachCollider();
            m_ColliderRadius = EditorGUILayout.FloatField("Radius", m_ColliderRadius);

            SetValues();
            EditorUtility.SetDirty(m_Targetter);
            EditorUtility.SetDirty(m_AttachedCollider);
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// For attaching and hiding the correct collider
        /// </summary>
        void AttachCollider()
        {
            if (m_AttachedCollider is SphereCollider)
            {
                GetValues();
                return;
            }
            if (m_AttachedCollider != null)
            {
                DestroyImmediate(m_AttachedCollider, true);
            }
            m_AttachedCollider = m_Targetter.gameObject.AddComponent<SphereCollider>();
            m_SerializedAttachedCollider.objectReferenceValue = m_AttachedCollider;

            SetValues();
            m_AttachedCollider.hideFlags = HideFlags.HideInInspector;
        }

        /// <summary>
        /// Assigns the values to the collider
        /// </summary>
        void SetValues()
        {
            var sphere = (SphereCollider)m_AttachedCollider;
            sphere.radius = m_ColliderRadius;
        }

        /// <summary>
        /// Obtains the information from the collider
        /// </summary>
        void GetValues()
        {
            var sphere = (SphereCollider)m_AttachedCollider;
            m_ColliderRadius = sphere.radius;
        }

        /// <summary>
        /// Caches the collider and hides it
        /// and configures all the necessary information from it
        /// </summary>
        void OnEnable()
        {
            m_Targetter = (Targetter)target;
            m_SerializedAttachedCollider = serializedObject.FindProperty("attachedCollider");
            m_AttachedCollider = (Collider)m_SerializedAttachedCollider.objectReferenceValue;

            if (m_AttachedCollider == null)
            {
                m_AttachedCollider = m_Targetter.GetComponent<Collider>();
                if (m_AttachedCollider == null)
                {
                    m_AttachedCollider = m_Targetter.gameObject.AddComponent<SphereCollider>();
                    m_SerializedAttachedCollider.objectReferenceValue = m_AttachedCollider;
                }
            }

            // to ensure the collider is referenced by the serialized object
            if (m_SerializedAttachedCollider.objectReferenceValue == null)
            {
                m_SerializedAttachedCollider.objectReferenceValue = m_AttachedCollider;
            }
            GetValues();
            m_AttachedCollider.isTrigger = true;
            m_AttachedCollider.hideFlags = HideFlags.HideInInspector;
        }
    }
}