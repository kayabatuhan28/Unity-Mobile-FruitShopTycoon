using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BuildArea : MonoBehaviour
{
    [SerializeField] int fieldID;               
    [SerializeField] int slotID;               
    public int setupCost;             
    public bool isOpen;                          

    [SerializeField] Image setupProgressSlider; 
    public GameObject setupObject;   
    [SerializeField] TextMeshProUGUI costText;  

    [SerializeField] GameObject treeObject;     
    [SerializeField] GameObject noMoneyObject;  

    Coroutine IE_StartBuild;

    public void UpdateCostText(int newCost, bool State)
    {
        setupCost = newCost;
        isOpen = State;
        costText.text = setupCost.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (setupCost <= GameManager.instance.Money)
            {             
                IE_StartBuild = StartCoroutine(StartBuild());
            }
            else
            {
                StartCoroutine(NotEnoughMoney());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!isOpen)
            {
                setupProgressSlider.fillAmount = 0f;
                if (IE_StartBuild != null)
                {
                    StopCoroutine(IE_StartBuild);
                    IE_StartBuild = null;
                }              
            }           
        }
    }

    IEnumerator StartBuild()
    {
        while (true)
        {
            setupProgressSlider.fillAmount += 0.1f;
            if (setupProgressSlider.fillAmount == 1)
            {
                isOpen = true;
                GameManager.instance.UpdateMoney(-setupCost);
                treeObject.SetActive(true);
                setupObject.SetActive(false);

                GameManager.instance.PlaySound(2);

                GameSaveSystem saveSystem = GameManager.instance._GameSaveSystem;
                saveSystem.SetFieldIsOpen(fieldID, slotID, true);

                if (IE_StartBuild != null)
                {
                    StopCoroutine(IE_StartBuild);
                    IE_StartBuild = null;
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator NotEnoughMoney()
    {
        noMoneyObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        noMoneyObject.SetActive(false);
    }


}
