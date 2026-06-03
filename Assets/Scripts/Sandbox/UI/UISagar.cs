using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Signs
{
    public class UISagar : MonoBehaviour
    {
        public Image vidaImg;
        public Sprite vidaCompleteSprite, vidaMediaSprite, vidaAcabandoSprite;
        public List<Image> feImgList = new();

        public void SetVidaImage(int vida)
        {
            if (vida >= 66 && vida <= 100)
            {
                vidaImg.sprite = vidaCompleteSprite;
            }
            else if (vida >= 36 && vida <= 65)
            {
                vidaImg.sprite = vidaMediaSprite;
            }
            else if (vida >= 0 && vida <= 35)
            {
                vidaImg.sprite = vidaAcabandoSprite;
            }
        }
    }
}
