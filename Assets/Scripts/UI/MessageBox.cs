using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class MessageBox: MonoBehaviour
    {
        public TextMeshProUGUI TitleLabel;
        public TextMeshProUGUI DescriptionLabel;
        public event EventHandler Appeared;
        public event EventHandler Disappeared;

        public void Show(string title, string message)
        {
            TitleLabel.text = title;
            DescriptionLabel.text = message;
            GetComponent<Animation>().PlayQueued("MessageBoxAppear");
        }

        public void Hide()
        {
            GetComponent<Animation>().PlayQueued("MessageBoxDisappear");
        }

        public void OnAppeared()
        {
            Appeared?.Invoke(this, new EventArgs());
        }

        public void OnDisappeared()
        {
            Disappeared?.Invoke(this, new EventArgs());
        }



    }
}
