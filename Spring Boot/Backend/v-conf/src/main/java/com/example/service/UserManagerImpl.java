package com.example.service;

import java.util.Optional;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;
import org.springframework.security.crypto.password.PasswordEncoder;

import com.example.auth.JwtUtil;
import com.example.dto.LoginRequest;
import com.example.models.User;
import com.example.repository.UserRepository;

@Service
public class UserManagerImpl implements UserManager {

    @Autowired
    private UserRepository userRepository;

    @Autowired
    private JwtUtil jwtUtil;

    @Autowired
    private PasswordEncoder passwordEncoder;

    @Override
    public String login(LoginRequest request) {

        User user = userRepository.findByUsername(request.getUsername())
                .orElseThrow(() -> new RuntimeException("Invalid username or password"));

        if (user.isBlocked()) {
            throw new RuntimeException("User is blocked");
        }

        if (!passwordEncoder.matches(request.getPassword(), user.getPassword())) {

            user.setFailedAttempts(user.getFailedAttempts() + 1);

            if (user.getFailedAttempts() >= 3) {
                user.setBlocked(true);
            }

            userRepository.save(user);
            throw new RuntimeException("Invalid username or password");
        }

        user.setFailedAttempts(0);
        userRepository.save(user);

        return jwtUtil.generateToken(user.getUsername());
    }
}
