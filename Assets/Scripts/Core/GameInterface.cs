using UnityEngine;
using TMPro;

namespace ExpertCenTest.Core
{
    public class GameInterface : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField agentsNumberInputField;
        [SerializeField]
        private TMP_InputField agentsEscapeInputField;
        [SerializeField]
        private TextMeshProUGUI agentsNumberText;

        public delegate void CreateAgentsHandle(int agentsNumber);
        public CreateAgentsHandle CreateAgentsEvent;

        public delegate void DestroyAgentsHandle();
        public DestroyAgentsHandle DestroyAgentsEvent;

        public delegate void AgentsEscapeChangeHandle(int escapeDistance);
        public AgentsEscapeChangeHandle AgentsEscapeChangeEvent;

        public delegate void InvertAllAgentsHandle();
        public InvertAllAgentsHandle InvertAllAgentsEvent;

        public void Initialize(AgentsController agentsController)
        {
            agentsController.UpdateAgentsNumberEvent += OnUpdateAgentsNumberEvent;
        }

        private void Start()
        {
            AgentsEscapeChangeEvent?.Invoke(int.Parse(agentsEscapeInputField.text));
        }

        public void OnCreateAgentsButtonClick()
        {
            CreateAgentsEvent?.Invoke(int.Parse(agentsNumberInputField.text));
        }

        public void OnDestroyAgentsButtonClick()
        {
            DestroyAgentsEvent?.Invoke();
        }

        public void OnInvertAgentsButtonClick()
        {
            InvertAllAgentsEvent?.Invoke();
        }

        public void OnExitButtonClick()
        {
            Application.Quit();
        }

        //≈сли значение в поле agentsEscapeInputField изменилось, то вызываем ивент с его значением, на который подписан AgentsController
        public void OnAgentsEscapeInputFieldValueChanged()
        {
            if (agentsEscapeInputField.text != "")
            {
                AgentsEscapeChangeEvent?.Invoke(int.Parse(agentsEscapeInputField.text));
            }
        }

        //¬ ответ на ивент об изменении количества агентов в списках выводим информацию в соответствующий text
        private void OnUpdateAgentsNumberEvent(int hunterAgentsCount, int wildAgentsCount)
        {
            agentsNumberText.text = "Hunters: " + hunterAgentsCount + ", Wilds: " + wildAgentsCount;
        }
    }
}