package com.example.controller;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.security.core.Authentication;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import com.example.dto.InvoiceRequestDTO;
import com.example.service.Invoicemanager;

@RestController
@RequestMapping("/api/invoice")
public class InvoiceController {

    @Autowired
    private Invoicemanager invoiceService;

    @PostMapping("/confirm")
    public ResponseEntity<String> confirmOrder(
            @RequestBody InvoiceRequestDTO dto,
            Authentication authentication
    ) {
        if (authentication == null || authentication.getName() == null) {
            throw new RuntimeException("User not authenticated");
        }

        String username = authentication.getName(); // ✅ SAFE
        invoiceService.generateInvoice(dto, username);

        return ResponseEntity.ok("Invoice generated");
    }


}
