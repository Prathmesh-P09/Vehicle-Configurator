//package com.example.service;
//
//import java.util.List;
//
//import org.springframework.beans.factory.annotation.Autowired;
//import org.springframework.stereotype.Service;
//import org.springframework.web.client.RestTemplate;
//
//import com.example.models.User;
//import com.example.repository.UserRepository;
//
//@Service
//public class UserServiceImpl implements UserService {
//
//    @Autowired
//    private UserRepository repository;
//
//    @Autowired
//    private EmailService emailService;
//
//    @Autowired
//    private RestTemplate restTemplate;
//
//    public List<User> getAllRegistrations() {
//        return repository.findAll();
//    }
//
//    @Override
//    public User saveRegistration(User user) {
//
//        String regNo = "VCONF-" + System.currentTimeMillis();
//        user.setRegistrationNo(regNo);
//        user.setRole("USER");
//
//        // 1️⃣ Save user in DB
//        User savedUser = repository.save(user);
//
//        // 2️⃣ CALL PDF MICROSERVICE (IMPORTANT)
//        byte[] pdfBytes = restTemplate.postForObject(
//                "http://localhost:9003/api/registration/pdf",
//                savedUser,
//                byte[].class
//        );
//
//        // 3️⃣ Send email with PDF
//        emailService.sendRegistrationEmail(savedUser, pdfBytes);
//
//        return savedUser;
//    }
//
//    @Override
//    public User getById(Integer id) {
//        return repository.findById(id)
//                .orElseThrow(() -> new RuntimeException("User not found"));
//    }
//}


package com.example.service;



import java.util.List;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import com.example.models.User;
import com.example.repository.UserRepository;

@Service
public class UserServiceImpl implements UserService {

    @Autowired
    private UserRepository repository;

    @Autowired
    private EmailService emailService;

    @Override
    public List<User> getAllRegistrations() {
        return repository.findAll();
    }

    // ✅ MAIN REGISTRATION METHOD
    @Override
    public User saveRegistration(User user) {

        /* =====================================================
         * STEP 1: SERVICE-LAYER VALIDATION (SAFE, NO DB CHANGE)
         * ===================================================== */

        // 1️⃣ Email duplicate check
        if (repository.existsByEmail(user.getEmail())) {
            throw new RuntimeException("Email already registered");
        }

        // 2️⃣ Username duplicate check
        if (repository.existsByUsername(user.getUsername())) {
            throw new RuntimeException("Username already exists");
        }

        // 3️⃣ Phone duplicate check (optional null safety)
        if (user.getPhone() != null &&
            repository.existsByPhone(user.getPhone())) {
            throw new RuntimeException("Phone number already registered");
        }

        /* =====================================================
         * STEP 2: BUSINESS LOGIC (YOUR EXISTING CODE)
         * ===================================================== */

        String regNo = "VCONF-" + System.currentTimeMillis();
        user.setRegistrationNo(regNo);
        user.setRole("USER");

        // ✅ SAVE ONLY AFTER VALIDATION PASSES
        User savedUser = repository.save(user);

        /* =====================================================
         * STEP 3: POST-SAVE OPERATIONS
         * ===================================================== */

        // ✅ Generate PDF
        byte[] pdfBytes =
                com.example.util.PdfGeneratorUtil.generateUserPdf(savedUser);

        // ✅ Send email WITH PDF
        emailService.sendRegistrationEmail(savedUser, pdfBytes);

        return savedUser;
    }

    @Override
    public User getById(Integer id) {
        return repository.findById(id)
                .orElseThrow(() -> new RuntimeException("User not found"));
    }
}