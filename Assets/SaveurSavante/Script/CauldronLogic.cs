using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CauldronLogic : MonoBehaviour
{
    [Header("Configuration Recette")]
    // La liste des tags qu'il faut mettre pour réussir
    public List<string> recipeTagsRequired;
    private List<string> currentIngredients = new List<string>();

    [Header("Effets Visuels & Liquide")]
    public GameObject splashEffect;
    public Renderer liquidRenderer; // <--- C'EST CETTE LIGNE QUI FAIT APPARAITRE LA CASE
    public Color successColor = Color.green;

    [Header("Sons")]
    public AudioSource audioSource;
    public AudioClip plantSound;
    public AudioClip jarSound;
    public AudioClip successSound;

    private bool isRecipeComplete = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isRecipeComplete) return;

        // Si l'objet a un tag présent dans notre liste de recette
        if (recipeTagsRequired.Contains(other.tag))
        {
            AddIngredient(other.gameObject);
        }
        // Sinon, si c'est une jarre
        else if (other.CompareTag("Jar"))
        {
            PlaySound(jarSound);
            other.gameObject.SetActive(false);
        }
    }

    void AddIngredient(GameObject ingredient)
    {
        Debug.Log("Ingrédient ajouté : " + ingredient.tag);

        currentIngredients.Add(ingredient.tag);
        PlaySound(plantSound);

        if (splashEffect != null)
            Instantiate(splashEffect, ingredient.transform.position, Quaternion.identity);

        ingredient.SetActive(false);

        CheckRecipe();
    }

    void CheckRecipe()
    {
        // On vérifie si on a atteint le nombre d'ingrédients requis
        if (currentIngredients.Count >= recipeTagsRequired.Count)
        {
            isRecipeComplete = true;
            CompletePotion();
        }
    }

    void CompletePotion()
    {
        Debug.Log("Potion réussie !");

        // Change la couleur du disque de liquide
        if (liquidRenderer != null)
        {
            liquidRenderer.material.color = successColor;
        }

        // Joue le son de victoire
        PlaySound(successSound);
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}