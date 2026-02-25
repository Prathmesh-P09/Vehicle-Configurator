package com.example.service;

import java.time.LocalDate;
import java.util.ArrayList;
import java.util.List;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import com.example.dto.AlternateComponentDTO;
import com.example.dto.InvoiceRequestDTO;
import com.example.models.Component;
import com.example.models.InvoiceDetail;
import com.example.models.InvoiceHeader;
import com.example.models.InvoiceStatus;
import com.example.models.Model;
import com.example.models.User;
import com.example.repository.ComponentRepository;
import com.example.repository.InvoiceDetailRepository;
import com.example.repository.InvoiceHeaderRepository;
import com.example.repository.ModelRepository;
import com.example.repository.UserRepository;

import jakarta.transaction.Transactional;

@Service
public class InvoiceManagerImpl implements Invoicemanager {

    @Autowired
    private ModelRepository modelRepository;

    @Autowired
    private InvoiceHeaderRepository invoiceRepo;

    @Autowired
    private UserRepository userRepository;

    @Autowired
    private InvoiceDetailRepository detailRepository;

    @Autowired
    private ComponentRepository componentRepo;

    @Autowired
    private InvoicePdfService pdfService;

    @Autowired
    private EmailServiceImpl emailService;

    @Override
    @Transactional
    public void generateInvoice(InvoiceRequestDTO dto, String username) {

        System.out.println("INVOICE DTO = " + dto);
        System.out.println("JWT USER = " + username);

        if (dto.getModelId() == null) {
            throw new RuntimeException("Model ID missing");
        }

        if (dto.getQty() == null || dto.getQty() <= 0) {
            throw new RuntimeException("Invalid quantity");
        }

        // ===== USER =====
        User user = userRepository.findByUsername(username)
                .or(() -> userRepository.findByEmail(username))
                .orElseThrow(() -> new RuntimeException("User not found"));

        // ===== MODEL =====
        Model model = modelRepository.findById(dto.getModelId())
                .orElseThrow(() -> new RuntimeException("Model not found"));

        // ===== PRICE CALC =====
        double baseAmt = model.getPrice() * dto.getQty();

        double addOnTotal = 0.0;
        List<Component> selectedComponents = new ArrayList<>();

        if (dto.getComponents() != null && !dto.getComponents().isEmpty()) {
            for (AlternateComponentDTO c : dto.getComponents()) {

                Component altComp = componentRepo.findById(c.getAltCompId())
                        .orElseThrow(() -> new RuntimeException("Alt component not found"));

                selectedComponents.add(altComp);
                addOnTotal += altComp.getPrice();
            }
        }

        double amount = baseAmt + addOnTotal;
        double tax = amount * 0.18;
        double totalAmt = amount + tax;

        // ===== SAVE HEADER =====
        InvoiceHeader invoice = new InvoiceHeader();
        invoice.setUser(user);
        invoice.setModel(model);
        invoice.setQty(dto.getQty());
        invoice.setBaseAmt(baseAmt);
        invoice.setTax(tax);
        invoice.setTotalAmt(totalAmt);
        invoice.setInvDate(LocalDate.now());
        invoice.setStatus(InvoiceStatus.Confirmed);
        invoice.setCustomerDetail(dto.getCustomerDetail());

        InvoiceHeader savedInvoice = invoiceRepo.save(invoice);

        // ===== SAVE DETAILS (ONLY USER SELECTION) =====
        for (Component comp : selectedComponents) {
            InvoiceDetail d = new InvoiceDetail();
            d.setInv(savedInvoice);
            d.setComp(comp);
            d.setCompPrice(comp.getPrice());
            detailRepository.save(d);
        }

        // ===== FETCH DETAILS =====
        List<InvoiceDetail> details =
                detailRepository.findAll().stream()
                        .filter(d -> d.getInv().getId().equals(savedInvoice.getId()))
                        .toList();

        // ===== PDF + EMAIL =====
        try {
            byte[] pdfBytes = pdfService.generateInvoicePdf(savedInvoice, details);
            System.out.println("📄 PDF generated, size = " + pdfBytes.length);

            System.out.println("📧 Sending invoice email...");
            emailService.sendInvoiceEmail(user, pdfBytes, savedInvoice.getId());
            System.out.println("✅ Invoice email sent");

        } catch (Exception e) {
            e.printStackTrace();
            throw new RuntimeException("Invoice generation failed");
        }
    }
}
