using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // <-- Obligatoire pour recharger la scène

public class CauldronLogic : MonoBehaviour
{
    [Header("Configuration Recette")]
    public List<string> recipeTagsRequired;
    private int currentStep = 0;

    [Header("Effets Visuels & Liquide")]
    public GameObject splashEffect;
    public Renderer liquidRenderer;
    public Color successColor = Color.green;
    public Color failureColor = Color.red;

    [Header("Sons")]
    public AudioSource audioSource;
    public AudioClip plantSound;
    public AudioClip successSound;
    public AudioClip errorSound;

    private bool isFinished = false;

    void Update()
    {
        // Si le joueur appuie sur R, on recommence tout
        InputDevice device = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

        // On cherche si le bouton "PrimaryButton" (X) ou "SecondaryButton" (Y) est pressé
        if (device.TryGetFeatureValue(CommonUsages.secondaryButton, out bool isPressed) && isPressed)
        {
            ResetScene();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isFinished) return;

        if (other.CompareTag("Jar"))
        {
            other.gameObject.SetActive(false);
            return;
        }

        // Si l'ingrédient correspond à l'étape actuelle de la liste
        if (currentStep < recipeTagsRequired.Count && other.CompareTag(recipeTagsRequired[currentStep]))
        {
            CorrectIngredient(other.gameObject);
        }
        else
        {
            // Si on se trompe de plante ou d'ordre
            WrongIngredient(other.gameObject);
        }
    }

    void CorrectIngredient(GameObject ingredient)
    {
        currentStep++;
        PlaySound(plantSound);
        if (splashEffect != null) Instantiate(splashEffect, ingredient.transform.position, Quaternion.identity);
        ingredient.SetActive(false);

        if (currentStep >= recipeTagsRequired.Count)
        {
            CompletePotion();
        }
    }

    void WrongIngredient(GameObject ingredient)
    {
        isFinished = true;
        PlaySound(errorSound);
        if (liquidRenderer != null) liquidRenderer.material.color = failureColor;
        ingredient.SetActive(false);
        Debug.Log("Raté ! Appuie sur 'R' pour réessayer.");
    }

    void CompletePotion()
    {
        isFinished = true;
        if (liquidRenderer != null) liquidRenderer.material.color = successColor;
        PlaySound(successSound);
        Debug.Log("Succès ! Appuie sur 'R' pour une nouvelle potion.");
    }

    public void ResetScene()
    {
        // Cette ligne recharge la scène active (celle où tu es)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null) audioSource.PlayOneShot(clip);
    }
}