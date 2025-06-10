using System;
using FoodyGo.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace FoodyGo.UI
{
    public class UI_BattleConfirmWindow : UI_Base
    {
        [SerializeField] Button _confirmButton;
        [SerializeField] Button _cancelButton;

        public void Start()
        {
            _confirmButton.onClick.AddListener(() =>
            {
                GameManager.instance.ActiveAdditiveScene("Catch");
                Hide();
            });
            
            _cancelButton.onClick.AddListener(() =>
            {
                Hide();
            });
        }


    }    
    
}


