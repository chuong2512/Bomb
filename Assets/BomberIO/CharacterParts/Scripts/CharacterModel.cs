﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterModel : MonoBehaviour
{
    public Transform rightHandContainer;
    public Transform leftHandContainer;
    public Transform shieldContainer;
    public Transform headContainer;
    public Transform[] customContainers;
    public Animator CacheAnimator { get; private set; }
    private GameObject headModel;
    private readonly List<GameObject> weaponModels = new List<GameObject>();
    private readonly Dictionary<int, GameObject> customModels = new Dictionary<int, GameObject>();
    private Transform rightDamageLaunchTransform;
    private Transform leftDamageLaunchTransform;

    private readonly Dictionary<int, Transform> customModelContainers = new Dictionary<int, Transform>();
    public Dictionary<int, Transform> CustomModelContainers
    {
        get
        {
            if (customModelContainers.Count != customContainers.Length)
            {
                customModelContainers.Clear();
                for (var i = 0; i < customContainers.Length; ++i)
                {
                    var customContainer = customContainers[i];
                    customModelContainers[i] = customContainer;
                }
            }
            return customModelContainers;
        }
    }

    private void Awake()
    {
        CacheAnimator = GetComponent<Animator>();
    }

    public void SetHeadModel(GameObject model)
    {
        if (headModel != null)
            Destroy(headModel);
        headModel = AddModel(model, headContainer, null);
    }

    public void SetWeaponModel(GameObject rightHandModel, GameObject leftHandModel, GameObject shieldModel)
    {
        ClearGameObjects(weaponModels);
        var newRightHandModel = AddModel(rightHandModel, rightHandContainer, weaponModels);
        var newLeftHandModel = AddModel(leftHandModel, leftHandContainer, weaponModels);
        AddModel(shieldModel, shieldContainer, weaponModels);
        // Set damage launch transforms
        rightDamageLaunchTransform = null;
        leftDamageLaunchTransform = null;
        if (newRightHandModel != null)
        {
            var comp = newRightHandModel.GetComponent<WeaponDamageLaunchTransform>();
            if (comp != null && comp.damageLaunchTransform != null)
                rightDamageLaunchTransform = comp.damageLaunchTransform;
        }
        if (newLeftHandModel != null)
        {
            var comp = newLeftHandModel.GetComponent<WeaponDamageLaunchTransform>();
            if (comp != null && comp.damageLaunchTransform != null)
                leftDamageLaunchTransform = comp.damageLaunchTransform;
        }
    }

    public void ClearCustomModels()
    {
        foreach (var model in customModels.Values)
        {
            Destroy(model);
        }
        customModels.Clear();
    }

    public void SetCustomModel(int position, GameObject model)
    {
        if (!CustomModelContainers.ContainsKey(position))
            return;

        GameObject oldModel = null;
        if (customModels.TryGetValue(position, out oldModel))
        {
            if (oldModel != null)
                Destroy(oldModel);
            customModels.Remove(position);
        }
        customModels[position] = AddModel(model, CustomModelContainers[position], null);
    }

    private void ClearGameObjects(List<GameObject> list)
    {
        foreach (var entry in list)
            Destroy(entry);
        list.Clear();
    }

    private GameObject AddModel(GameObject model, Transform transform, List<GameObject> list)
    {
        if (model == null)
            return null;
        var newModel = Instantiate(model);
        newModel.transform.parent = transform;
        newModel.transform.localPosition = Vector3.zero;
        newModel.transform.localEulerAngles = Vector3.zero;
        newModel.transform.localScale = Vector3.one;
        newModel.gameObject.SetActive(true);
        if (list != null)
            list.Add(newModel);
        return newModel;
    }

    public bool TryGetDamageLaunchTransform(bool isLeftHand, out Transform launchTransform)
    {
        launchTransform = null;
        if (!isLeftHand && rightDamageLaunchTransform != null)
        {
            launchTransform = rightDamageLaunchTransform;
            return true;
        }
        else if (isLeftHand && leftDamageLaunchTransform != null)
        {
            launchTransform = leftDamageLaunchTransform;
            return true;
        }
        return false;
    }
}
