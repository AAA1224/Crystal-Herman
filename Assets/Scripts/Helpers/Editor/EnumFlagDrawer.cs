using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace KoboldTools
{

    [CustomPropertyDrawer(typeof(EnumFlagAttribute))]
    public class EnumFlagDrawer : PropertyDrawer
    {
        int enumIndex = 0;
        //private Dictionary<int, string> selectedNames = new Dictionary<int, string>();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.serializedObject.isEditingMultipleObjects)
            {
                EditorGUI.LabelField(position, "Multi object editing not allowed.");
                return;
            }


            EnumFlagAttribute flagSettings = (EnumFlagAttribute)attribute;
            bool convertToInt = flagSettings.convertToInt;
            string propName = property.name;

            if (!convertToInt)
            {
                //draw to enum         
                Enum targetEnum = GetBaseProperty<Enum>(property);
                EditorGUI.BeginProperty(position, label, property);
                Enum enumNew = EditorGUI.EnumFlagsField(position, propName, targetEnum);
                property.intValue = (int)Convert.ChangeType(enumNew, targetEnum.GetType());
                EditorGUI.EndProperty();
            }
            else
            {


                //draw to int
                int targetInt = GetBaseProperty<int>(property);
                

                string[] enumNames = GetEnumFlagsNames();
                if (enumNames.Length > 0)
                {
                    //get enum index from names in dictionary for selected enum
                    /*string selectedName;
                    if (selectedNames.TryGetValue(property.serializedObject.targetObject.GetInstanceID(), out selectedName))
                    {
                        Debug.Log("found selected name: " + selectedName);
                        Debug.Log("path: " + property.propertyPath);
                        for (int i = 0; i < enumNames.Length; i++)
                        {
                            if (enumNames[i] == selectedName)
                            {
                                Debug.Log("found index: " + enumIndex);
                                enumIndex = i;
                            }
                        }
                    }*/

                    EditorGUI.BeginProperty(position, label, property);
                    enumIndex = EditorGUI.Popup(new Rect(position.x,position.y,position.width,position.height/2f),"Get state from:", enumIndex, enumNames);

                    //create dictionary entry for selected enum
                    /*if (!selectedNames.ContainsKey(property.serializedObject.targetObject.GetInstanceID()))
                    {
                        selectedNames.Add(property.serializedObject.targetObject.GetInstanceID(), enumNames[enumIndex]);
                    }
                    else
                    {
                        selectedNames[property.serializedObject.targetObject.GetInstanceID()] = enumNames[enumIndex];
                    }*/


                    Type enumType = GetEnumType(enumNames[enumIndex]);
                    if (enumType == null)
                    {
                        throw new ArgumentException("Specified enum type could not be found", enumNames[enumIndex]);
                    }

                    Enum targetEnum = (Enum)Enum.ToObject(enumType, targetInt); // (Enum)Convert.ChangeType(targetInt, enumType);           
                    Enum enumNew = EditorGUI.EnumFlagsField(new Rect(position.x,position.y + position.height / 2f,position.width,position.height / 2f), propName, targetEnum);
                    property.intValue = (int)Convert.ChangeType(enumNew, targetEnum.GetType());
                    EditorGUI.EndProperty();
                }

            }


        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) *2f;
        }

        static T GetBaseProperty<T>(SerializedProperty prop)
        {
            // Separate the steps it takes to get to this property
            string[] separatedPaths = prop.propertyPath.Split('.');

            // Go down to the root of this serialized property
            System.Object reflectionTarget = prop.serializedObject.targetObject as object;
            // Walk down the path to get the target object
            foreach (var path in separatedPaths)
            {
                FieldInfo fieldInfo = reflectionTarget.GetType().GetField(path);
                reflectionTarget = fieldInfo.GetValue(reflectionTarget);
            }

            return (T)reflectionTarget;

        }

        static Type GetEnumType(string enumName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(enumName);
                if (type == null)
                    continue;
                if (type.IsEnum)
                    return type;
            }
            return null;
        }

        static string[] GetEnumFlagsNames()
        {
            List<string> names = new List<string>();


            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {

                foreach (Type t in assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(Enum)) && x.GetCustomAttributes(typeof(StateFlagsAttribute), false).Any()))
                {
                    if (t == null)
                        continue;
                    if (t.IsEnum)
                        names.Add(t.FullName);
                }

            }
            return names.ToArray();
        }

    }
}


