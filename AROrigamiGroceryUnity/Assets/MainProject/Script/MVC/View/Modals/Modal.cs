using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hsinpa.View
{
    public class Modal : BaseView
    {
        [SerializeField]
        private Button CloseBtn;

        /// <summary>
        /// Don't call this function, except from Modal.cs, Unless you are author
        /// </summary>
        /// <param name="isShow"></param>
        public override void Show(bool isShow)
        {
            base.Show(isShow);
        }

        protected virtual void Start()
        {
            SetCloseBtnEvent();
        }

        private void SetCloseBtnEvent()
        {
            if (CloseBtn != null)
            {
                CloseBtn.onClick.RemoveAllListeners();
                CloseBtn.onClick.AddListener(() => {
                    Modals.instance.Close();
                });
            }
        }
    }
}