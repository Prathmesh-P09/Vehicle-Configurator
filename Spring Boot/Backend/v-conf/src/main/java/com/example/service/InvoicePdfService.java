package com.example.service;

import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileOutputStream;
import java.util.List;

import org.springframework.stereotype.Service;

import com.example.models.InvoiceDetail;
import com.example.models.InvoiceHeader;
import com.itextpdf.text.Document;
import com.itextpdf.text.Paragraph;
import com.itextpdf.text.pdf.PdfPTable;
import com.itextpdf.text.pdf.PdfWriter;

@Service
public class InvoicePdfService {

	public byte[] generateInvoicePdf(
	        InvoiceHeader invoice,
	        List<InvoiceDetail> details) throws Exception {

	    ByteArrayOutputStream baos = new ByteArrayOutputStream();
	    Document document = new Document();
	    PdfWriter.getInstance(document, baos);

	    document.open();

	    document.add(new Paragraph("VEHICLE INVOICE"));
	    document.add(new Paragraph("Invoice ID: " + invoice.getId()));
	    document.add(new Paragraph("Date: " + invoice.getInvDate()));
	    document.add(new Paragraph("Customer: " + invoice.getCustomerDetail()));
	    document.add(new Paragraph(" "));

	    PdfPTable table = new PdfPTable(3);
	    table.addCell("Component");
	    table.addCell("Type");
	    table.addCell("Price");

	    for (InvoiceDetail d : details) {
	        table.addCell(d.getComp().getCompName());
	        table.addCell(d.getComp().getType());
	        table.addCell("₹ " + d.getCompPrice());
	    }

	    document.add(table);
	    document.add(new Paragraph(" "));
	    document.add(new Paragraph("Base Amount: ₹ " + invoice.getBaseAmt()));
	    document.add(new Paragraph("Tax: ₹ " + invoice.getTax()));
	    document.add(new Paragraph("Total: ₹ " + invoice.getTotalAmt()));

	    document.close();

	    return baos.toByteArray();
	}

}
