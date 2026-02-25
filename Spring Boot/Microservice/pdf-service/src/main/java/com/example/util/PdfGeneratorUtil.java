package com.example.util;





import com.example.models.User;
import com.itextpdf.text.*;
import com.itextpdf.text.pdf.PdfWriter;

import java.io.ByteArrayOutputStream;

public class PdfGeneratorUtil {

    public static byte[] generateUserPdf(User user) {

        ByteArrayOutputStream out = new ByteArrayOutputStream();

        try {
            Document document = new Document(PageSize.A4, 36, 36, 36, 36);
            PdfWriter.getInstance(document, out);
            document.open();

            // ===== FONTS =====
            Font titleFont = new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD);
            Font sectionFont = new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD);
            Font labelFont = new Font(Font.FontFamily.HELVETICA, 11, Font.BOLD);
            Font valueFont = new Font(Font.FontFamily.HELVETICA, 11);

            // ===== TITLE =====
            document.add(new Paragraph("User Registration Details", titleFont));
            document.add(Chunk.NEWLINE);

            // ===== COMPANY DETAILS =====
            document.add(new Paragraph("Company Details", sectionFont));
            document.add(Chunk.NEWLINE);

            addField(document, labelFont, valueFont, "Company Name", user.getCompanyName());
            addField(document, labelFont, valueFont, "Holding Type", user.getHoldingType());
            addField(document, labelFont, valueFont, "Company ST No", user.getCompanyStNo());
            addField(document, labelFont, valueFont, "Company VAT No", user.getCompanyVatNo());
            addField(document, labelFont, valueFont, "PAN Number", user.getTaxPan());
            addField(document, labelFont, valueFont, "Registration No", user.getRegistrationNo());

            document.add(Chunk.NEWLINE);

            // ===== AUTHORIZED PERSON =====
            document.add(new Paragraph("Authorized Person Details", sectionFont));
            document.add(Chunk.NEWLINE);

            addField(document, labelFont, valueFont, "Name", user.getAuthName());
            addField(document, labelFont, valueFont, "Designation", user.getDesignation());
            addField(document, labelFont, valueFont, "Auth Telephone", user.getAuthTel());
            addField(document, labelFont, valueFont, "Mobile", user.getCell());
            addField(document, labelFont, valueFont, "Phone", user.getPhone());
            addField(document, labelFont, valueFont, "Email", user.getEmail());

            document.add(Chunk.NEWLINE);

            // ===== ADDRESS =====
            document.add(new Paragraph("Address Details", sectionFont));
            document.add(Chunk.NEWLINE);

            addField(document, labelFont, valueFont, "Address Line 1", user.getAdd1());
            addField(document, labelFont, valueFont, "Address Line 2", user.getAdd2());
            addField(document, labelFont, valueFont, "City", user.getCity());
            addField(document, labelFont, valueFont, "State", user.getState());
            addField(document, labelFont, valueFont, "PIN Code", user.getPin());
            addField(document, labelFont, valueFont, "Telephone", user.getTel());
            addField(document, labelFont, valueFont, "Fax", user.getFax());

            document.add(Chunk.NEWLINE);

            // ===== ACCOUNT DETAILS =====
            document.add(new Paragraph("Account Details", sectionFont));
            document.add(Chunk.NEWLINE);

            addField(document, labelFont, valueFont, "Username", user.getUsername());
            addField(document, labelFont, valueFont, "Role", user.getRole());

            // ❌ PASSWORD IS NOT ADDED (SECURITY ✔)

            document.close();

        } catch (Exception e) {
            e.printStackTrace();
        }

        return out.toByteArray();
    }

    // ===== HELPER METHOD =====
    private static void addField(
            Document document,
            Font labelFont,
            Font valueFont,
            String label,
            String value
    ) throws DocumentException {

        if (value == null || value.trim().isEmpty()) return;

        Paragraph p = new Paragraph();
        p.add(new Chunk(label + ": ", labelFont));
        p.add(new Chunk(value, valueFont));
        p.setSpacingAfter(6f);
        document.add(p);
    }
}