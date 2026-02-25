package com.example.service;




import java.io.File;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import com.example.models.User;

import jakarta.mail.internet.MimeMessage;

@Service
public class EmailServiceImpl implements EmailService {

    @Autowired
    private EmailSender mailSender;
    @Autowired
    private InvoicePdfService pdfService;

    

    @Override
    public void sendRegistrationEmail(User user, byte[] pdfBytes) {

        String subject = "Registration Successful - PDF Attached";

        String message =
                "Hello " + user.getAuthName() + ",\n\n" +
                "Your registration has been completed successfully.\n\n" +
                "Registration No: " + user.getRegistrationNo() + "\n" +
                "Company: " + user.getCompanyName() + "\n\n" +
                "Please find your registration PDF attached.\n\n" +
                "Regards,\nVehicle Configurator Team";

        mailSender.sendWithAttachment(
                user.getEmail(),
                subject,
                pdfBytes,
                "registration-details.pdf",
                message
        );
    }
    public void sendInvoiceEmail(
            User user,
            byte[] pdfBytes,
            Integer invoiceId
    ) {
        System.out.println("📧 SENDING INVOICE EMAIL");
        System.out.println("To: " + user.getEmail());
        System.out.println("Invoice ID: " + invoiceId);
        System.out.println("PDF size: " + pdfBytes.length);

        String subject = "Invoice #" + invoiceId;

        String message =
                "Hello " + user.getAuthName() + ",\n\n" +
                "Thank you for your vehicle order.\n\n" +
                "Invoice No: " + invoiceId + "\n\n" +
                "Please find the invoice PDF attached.\n\n" +
                "Regards,\nVehicle Configurator Team";

        mailSender.sendWithAttachment(
                user.getEmail(),
                subject,
                pdfBytes,
                "invoice_" + invoiceId + ".pdf",
                message
        );

        System.out.println("✅ EMAIL METHOD FINISHED");
    }

    
}