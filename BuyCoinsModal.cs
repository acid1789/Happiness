using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HappinessNetwork;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Happiness
{
    class BuyCoinsModal
    {
        enum PurchaseState
        {
            FetchingProducts,
            SelectingProduct,
            ConfirmingPurchase,
            PurchasingProduct,
            PurchaseComplete
        }
        PurchaseState m_State;

        Rectangle m_Rect;
        Rectangle m_WaitRect;
        Rectangle m_WaitRectPurchase;


        UIButton m_CancelButton;

        SW_RequestProductsJob m_ProductsRequest;
        SW_VerifyPurchaseJob m_VerifyPurchase;
        List<ProductDisplay> m_Products;
        UIFrame m_ScrollFrame;
        Rectangle m_ScrollRect;
        float m_ScrollPosition;
        float m_DragY;
        float m_ScrollMax;

        int m_iSelectedProduct;
        int m_iConfirmProductY;
        UILabel m_ConfirmText;
        UILabel m_ThankYouText;
        UIButton m_PurchaseButton;

        UILabel[] m_PurchaseWaitMessages;

        public BuyCoinsModal()
        {
            m_Products = new List<ProductDisplay>();

            Happiness game = Happiness.Game;
            int screenWidth = game.ScreenWidth;
            int screenHeight = game.ScreenHeight;

            int centerDialogX = (screenWidth >> 1);
            int centerY = (screenHeight >> 1);

            int width = (int)(Constants.BuyCreditsDialog_Width * screenWidth);
            int height = (int)(Constants.BuyCreditsDialog_Height * screenHeight);

            int left = centerDialogX - (width >> 1);
            int top = centerY - (height >> 1);
            m_Rect = new Rectangle(left, top, width, height);

            int waitIconSize = (int)(Constants.BuyCreditsDialog_WaitIconSize * game.ScreenWidth);
            m_WaitRect = new Rectangle((screenWidth / 2) - (waitIconSize / 2), (screenHeight / 2) - (waitIconSize / 2), waitIconSize, waitIconSize);
            
            int bottom = top + height;
            int marginTopBottom = (int)(Constants.BuyCreditsDialog_TopBottomMargin * screenHeight);
            int buttonWidth = (int)(Constants.BuyCreditsDialog_ButtonWidth * screenWidth);
            int buttonHeight = (int)(Constants.BuyCreditsDialog_ButtonHeight * screenHeight);
            int centerButtonLeft = centerDialogX - (buttonWidth >> 1);
            m_CancelButton = new UIButton(0, "Cancel", Assets.HelpFont, new Rectangle(centerButtonLeft, ((bottom - marginTopBottom) - buttonHeight), buttonWidth, buttonHeight), Assets.ScrollBar);
            m_CancelButton.ClickSound = SoundManager.SEInst.MenuCancel;

            m_PurchaseButton = new UIButton(0, "Purchase", Assets.HelpFont, new Rectangle(centerButtonLeft, m_CancelButton.Rect.Top - (marginTopBottom * 2) - buttonHeight, buttonWidth, buttonHeight), Assets.ScrollBar);


            int scrollWidth = (int)(Constants.BuyCreditsDialog_ProductWidth * screenWidth);
            int scrollTop = top + marginTopBottom;
            int scrollHeight = (m_CancelButton.Rect.Top - marginTopBottom) - scrollTop;
            m_ScrollPosition = 0;
            m_ScrollRect = new Rectangle(centerDialogX - (scrollWidth >> 1), scrollTop, scrollWidth, scrollHeight);
            m_ScrollFrame = new UIFrame(5, m_ScrollRect);

            m_iConfirmProductY = top + (marginTopBottom * 4);
            m_ConfirmText = new UILabel("Do you want to purchase this product?", centerDialogX, top + (marginTopBottom * 2), Color.Goldenrod, Assets.MenuFont, UILabel.XMode.Center);
            m_ThankYouText = new UILabel("Thank you for your purchase!", centerDialogX, top + (marginTopBottom * 2), Color.Goldenrod, Assets.MenuFont, UILabel.XMode.Center);

            waitIconSize >>= 1;
            m_WaitRectPurchase = new Rectangle((screenWidth / 2) - (waitIconSize >> 1), top + marginTopBottom + (waitIconSize >> 1), waitIconSize, waitIconSize);

            m_PurchaseWaitMessages = new UILabel[4];
            int textY = m_WaitRectPurchase.Bottom + (marginTopBottom * 2);
            m_PurchaseWaitMessages[0] = new UILabel("Waiting for purchase completion...", centerDialogX, textY, Color.Goldenrod, Assets.DialogFont, UILabel.XMode.Center);

            textY += (int)m_PurchaseWaitMessages[0].Height + marginTopBottom;
            m_PurchaseWaitMessages[1] = new UILabel("If you made a purchase and do not wish to wait,", centerDialogX, textY, Color.White, Assets.HelpFont, UILabel.XMode.Center);

            textY += (int)m_PurchaseWaitMessages[0].Height;
            m_PurchaseWaitMessages[2] = new UILabel("you can press cancel without interrupting your purchase.", centerDialogX, textY, Color.White, Assets.HelpFont, UILabel.XMode.Center);            

            textY += (int)m_PurchaseWaitMessages[0].Height;
            m_PurchaseWaitMessages[3] = new UILabel("Your coins and VIP points will be awarded when the transaction finishes processing.", centerDialogX, textY, Color.White, Assets.HelpFont, UILabel.XMode.Center);

            m_iSelectedProduct = -1;
            FetchProducts();
        }

        #region Input
        // Return false if this menu should close
        public bool HandleClick(int iX, int iY)
        {
            if (m_State == PurchaseState.SelectingProduct)
            {
                if (m_ScrollRect.Contains(iX, iY))
                {
                    foreach (ProductDisplay pd in m_Products)
                        pd.Selected = false;
                    int product = (int)((iY - (m_ScrollRect.Top - m_ScrollPosition)) / m_Products[0].Height);
                    m_Products[product].Selected = true;

                    if (m_Products[product].Button.Click(iX, iY))
                    {
                        PurchaseProduct(product);
                        return true;
                    }
                }
            }

            if (m_State == PurchaseState.ConfirmingPurchase &&
                (m_PurchaseButton.Click(iX, iY) || m_Products[m_iSelectedProduct].Button.Click(iX, iY)) )
            {
                DoPurchase();
            }

            if (m_CancelButton.Click(iX, iY))
            {
                if (m_State == PurchaseState.ConfirmingPurchase)
                {
                    m_State = PurchaseState.SelectingProduct;
                    return true;
                }
                return false;
            }
            return true;
        }
        public void DragBegin(DragArgs args)
        {
            if (m_State == PurchaseState.SelectingProduct)
                m_DragY = (float)args.StartY;
        }

        public void Drag(DragArgs args)
        {
            if (m_State == PurchaseState.SelectingProduct)
            {
                float deltaY = m_DragY - args.CurrentY;
                m_DragY = args.CurrentY;
                DoScroll(deltaY);
            }
        }

        public void Scroll(int delta)
        {
            if (m_State == PurchaseState.SelectingProduct)
            {
                float fDelta = (m_Products[0].Height * 1.5f) * ((delta < 0) ? 1.0f : -1.0f);
                DoScroll(fDelta);
            }
        }

        void DoScroll(float deltaY)
        {
            if (deltaY != 0)
            {
                m_ScrollPosition += deltaY;
                if (m_ScrollPosition < 0)
                    m_ScrollPosition = 0;
                if (m_ScrollPosition > m_ScrollMax)
                    m_ScrollPosition = m_ScrollMax;
            }
        }
        #endregion

        public void Update(GameTime gameTime)
        {
            if (m_State == PurchaseState.FetchingProducts)
            {
                if (m_ProductsRequest != null && m_ProductsRequest.Finished)
                {
                    SetupProducts(m_ProductsRequest.Products);
                    m_ProductsRequest.Destroy();
                    m_ProductsRequest = null;
                }
            }
            if (m_State == PurchaseState.PurchasingProduct)
            {
                if (m_VerifyPurchase != null && m_VerifyPurchase.Finished)
                {
                    m_State = PurchaseState.PurchaseComplete;
                    m_CancelButton.Text = "Close";
                    m_VerifyPurchase.Destroy();
                    m_VerifyPurchase = null;
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            // Draw frame & background
            sb.Draw(Assets.TransparentBox, m_Rect, Color.DarkGray);
            sb.Draw(Assets.TransparentBox, m_Rect, Color.DarkGray);
            sb.Draw(Assets.TransparentBox, m_Rect, Color.DarkGray);

            if( m_State == PurchaseState.SelectingProduct)
                DrawProducts(sb);
            else if( m_State == PurchaseState.ConfirmingPurchase )
                DrawConfirm(sb);
            else if( m_State == PurchaseState.PurchasingProduct )
                DrawPurchaseProduct(sb);
            else if( m_State == PurchaseState.PurchaseComplete )
                DrawPurchaseComplete(sb);
            else
                Assets.WaitIcon.Draw(sb, m_WaitRect, Color.White);

            m_CancelButton.Draw(sb);
        }

        void FetchProducts()
        {
            m_State = PurchaseState.FetchingProducts;
            m_ProductsRequest = Happiness.Game.ServerWriter.RequestProducts();
        }

        void SetupProducts(NetworkCore.GlobalProduct[] products)
        {
            m_State = PurchaseState.SelectingProduct;

            foreach (NetworkCore.GlobalProduct p in products)
            {
                ProductDisplay disp = new ProductDisplay(p, m_ScrollRect.Left, m_ScrollRect.Width);
                m_Products.Add(disp);
            }

            int productsHeight = (int)(m_Products.Count * m_Products[0].Height);
            m_ScrollMax = (m_ScrollRect.Height < productsHeight ) ? productsHeight - m_ScrollRect.Height : 0;
        }
        
        void PurchaseProduct(int product)
        {
            m_State = PurchaseState.ConfirmingPurchase;
            m_iSelectedProduct = product;
        }

        void DoPurchase()
        {
            // Set the state
            m_State = PurchaseState.PurchasingProduct;

            // Open the browser
            string host = "localhost";            
            string url = string.Format("http://{0}:8080/purchase?uid={1}&pid={2}", host, Happiness.Game.AccountId, m_Products[m_iSelectedProduct].ProductID);
            System.Diagnostics.Process.Start(url);

            // Querry the server for the credits/vip
            m_VerifyPurchase = Happiness.Game.ServerWriter.WaitForPurchaseComplete();
        }

        void DrawProducts(SpriteBatch sb)
        {
            Rectangle r = sb.GraphicsDevice.ScissorRectangle;
            sb.GraphicsDevice.ScissorRectangle = m_ScrollRect;
            float y = m_ScrollRect.Top - m_ScrollPosition;
            foreach (ProductDisplay p in m_Products)
            {
                p.Draw(sb, (int)y);
                y += p.Height;
            }
            sb.GraphicsDevice.ScissorRectangle = r;

            // Draw frame
            m_ScrollFrame.Draw(sb);
        }

        void DrawConfirm(SpriteBatch sb)
        {
            m_ConfirmText.Draw(sb);

            ProductDisplay pd = m_Products[m_iSelectedProduct];
            pd.Draw(sb, m_iConfirmProductY);

            m_PurchaseButton.Draw(sb);
        }

        void DrawPurchaseProduct(SpriteBatch sb)
        {
            Assets.WaitIcon.Draw(sb, m_WaitRectPurchase, Color.White);

            foreach( UILabel label in m_PurchaseWaitMessages )
                label.Draw(sb);
        }

        void DrawPurchaseComplete(SpriteBatch sb)
        {
            m_ThankYouText.Draw(sb);
            ProductDisplay pd = m_Products[m_iSelectedProduct];
            pd.Draw(sb, m_iConfirmProductY);
        }
    }
}
