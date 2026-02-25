package com.example.service;



import java.util.Properties;

import jakarta.activation.DataHandler;
import jakarta.activation.DataSource;
import jakarta.mail.*;
import jakarta.mail.internet.*;
import jakarta.mail.util.ByteArrayDataSource;

import org.springframework.stereotype.Component;

@Component
public class EmailSender {

    private final String username = "omkarbghatage03@gmail.com";
    private final String password = "omaw zzck chmt ivmv"; // app password

    public void sendWithAttachment(
            String toEmail,
            String subject,
            byte[] pdfBytes,
            String fileName,
            String messageText
    ) {
        try {
            Properties props = new Properties();
            props.put("mail.smtp.auth", "true");
            props.put("mail.smtp.starttls.enable", "true");
            props.put("mail.smtp.host", "smtp.gmail.com");
            props.put("mail.smtp.port", "587");

            Session session = Session.getInstance(props,
                new Authenticator() {
                    protected PasswordAuthentication getPasswordAuthentication() {
                        return new PasswordAuthentication(username, password);
                    }
                });

            Message message = new MimeMessage(session);
            message.setFrom(new InternetAddress(username));
            message.setRecipients(
                    Message.RecipientType.TO,
                    InternetAddress.parse(toEmail)
            );
            message.setSubject(subject);

            Multipart multipart = new MimeMultipart();

            // ===== TEXT BODY =====
            MimeBodyPart textPart = new MimeBodyPart();
            textPart.setText(messageText);
            multipart.addBodyPart(textPart);

            // ===== PDF ATTACHMENT =====
            MimeBodyPart attachmentPart = new MimeBodyPart();
            DataSource dataSource =
                    new ByteArrayDataSource(pdfBytes, "application/pdf");

            attachmentPart.setDataHandler(new DataHandler(dataSource));
            attachmentPart.setFileName(fileName);
            multipart.addBodyPart(attachmentPart);

            message.setContent(multipart);

            Transport.send(message);
            System.out.println("✅ Email sent to " + toEmail);

        } catch (Exception e) {
            e.printStackTrace();
        }
    }
}