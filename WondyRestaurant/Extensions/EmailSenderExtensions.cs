using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using WondyRestaurant.Services;
using WondyRestaurant.Utility;

namespace WondyRestaurant.Services
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "Confirm your email",
                $"Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>");
        }
        public static Task SendOrderStatusAsync( this IEmailSender emailSender, string email, string orderNumber, string orderStatus)
        {
            string subject = " ";
            string message = " ";
            
            if (orderStatus == SD.StatusCancelled)
            {
                subject = "Order Cancelled";
                message = "The order number " + orderNumber + "has ben cancelled Please contact us if you have any question";


            }
            if (orderStatus == SD.StatusSubmited)
            {
                subject = "Order created  Successfully ";
                message = "The order number " + orderNumber + "has ben created successfully";


            }
            if (orderStatus == SD.StatusInReady)
            {
                subject = "Order is ready for pickup";
                message = "The order number " + orderNumber + "is ready for pickup";

            }
            if (orderStatus == SD.StatusCompleted)
            {
                subject = "Order is ready for pickup";
                message = "The order number " + orderNumber + "is ready for pickup";

            }
            return emailSender.SendEmailAsync(email, subject, message);
        }
    }
}
