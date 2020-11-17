using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSkin : MonoBehaviour
{
    public MeshRenderer scarfMeshRenderer;
    public MeshRenderer shirtMeshRenderer;
    public MeshRenderer pantMeshRenderer;
    public SkinnedMeshRenderer scarf;
    public SkinnedMeshRenderer shirt;
    public SkinnedMeshRenderer pant;
    public GameObject[] masks;

    public Material[] scarfMats;
    public Material[] shirtMats;
    public Material[] pantMats;
    // Start is called before the first frame update
    void Start()
    {
        bool scarfBool = (Random.value > 0.5f);
        bool shirtBool = (Random.value > 0.5f);
        bool maskBool = (Random.value > 0.5f);

        if (shirt)
        {
            shirt.gameObject.SetActive(shirtBool);
        }

        if (shirtMeshRenderer)
        {
            shirtMeshRenderer.gameObject.SetActive(shirtBool);
        }

        if (scarf)
        {
            scarf.gameObject.SetActive(scarfBool);
        }

        if (scarfMeshRenderer)
        {
            scarfMeshRenderer.gameObject.SetActive(scarfBool);
        }

        if (masks.Length > 0)
        {
            if (scarfBool == false)
            {
                masks[Random.Range(0, masks.Length)].SetActive(maskBool);
            }
        }
        
        if(scarf)
        {
            scarf.material = scarfMats[Random.Range(0, scarfMats.Length)];
        }

        if(scarfMeshRenderer)
        {
            scarfMeshRenderer.material = scarfMats[Random.Range(0, scarfMats.Length)];
        }

        if (pant)
        {
            pant.material = pantMats[Random.Range(0, pantMats.Length)];
        }

        if (pantMeshRenderer)
        {
            pantMeshRenderer.material = pantMats[Random.Range(0, pantMats.Length)];
        }

        if (shirt)
        {
            shirt.material = shirtMats[Random.Range(0, shirtMats.Length)];
        }

        if (shirtMeshRenderer)
        {
            shirtMeshRenderer.material = shirtMats[Random.Range(0, shirtMats.Length)];
        }
        
        
        
    }
}
