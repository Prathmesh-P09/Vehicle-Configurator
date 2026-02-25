package com.example.service;
import org.springframework.http.HttpHeaders;
import org.springframework.http.MediaType;



import com.example.models.User;
import com.example.util.PdfGeneratorUtil;
import org.springframework.stereotype.Service;

@Service
public class RegistrationPdfService {

    public byte[] generatePdf(User user) {
        return PdfGeneratorUtil.generateUserPdf(user);
    }
}
