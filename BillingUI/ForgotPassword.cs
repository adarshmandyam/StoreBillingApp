﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AC.Billing.Business;

namespace BillingUI
{
    public partial class ForgotPassword : BaseClass
    {
        UserBLL userBLL;

        public ForgotPassword()
        {
            userBLL = new UserBLL(dbHelper);
            InitializeComponent();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtNewPassword.Text = string.Empty;
            txtUserName.Text = string.Empty;
            txtNewPasswordConfirm.Text = string.Empty;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtUserName.Text) || String.IsNullOrEmpty(txtNewPassword.Text))
            {
                lblValidationMessage.Text = "User Name or New Password not entered";
            }
            else if (txtNewPassword.Text != txtNewPasswordConfirm.Text)
            {
                lblValidationMessage.Text = "New Password and confirm new password fields do not match.";
            }
            else
            {
                lblValidationMessage.Text = "";
                int ret = userBLL.UpdatePassword(txtUserName.Text, txtNewPassword.Text);  //txtOldPassword.Text, 
                if (ret == 0)
                {
                    lblValidationMessage.Text = "Password Updated Successfully";
                }
                //else if (ret == 1)
                //{
                //    lblValidationMessage.Text = "Old Password not matching with the userID";
                //}
                else if (ret == 2)
                {
                    lblValidationMessage.Text = "User ID not found";
                }
                else
                {
                    lblValidationMessage.Text = "";
                }
            }
        }
    }
}
