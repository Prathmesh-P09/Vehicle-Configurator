package com.example.controller;

import com.example.models.User;
import com.example.service.RegistrationPdfService;
import org.springframework.http.HttpHeaders;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/api/registration/pdf")
@CrossOrigin(origins = "http://localhost:5173")
public class RegistrationPdfController {

    private final RegistrationPdfService pdfService;

    public RegistrationPdfController(RegistrationPdfService pdfService) {
        this.pdfService = pdfService;
    }

    @PostMapping
    public ResponseEntity<byte[]> generatePdf(@RequestBody User user) {

        byte[] pdfBytes = pdfService.generatePdf(user);

        return ResponseEntity.ok()
                .header(HttpHeaders.CONTENT_DISPOSITION,
                        "attachment; filename=registration-details.pdf")
                .contentType(MediaType.APPLICATION_PDF)
                .body(pdfBytes);
    }
}
