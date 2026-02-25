package com.example.auth;

import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.security.core.Authentication;
import org.springframework.security.oauth2.core.user.OAuth2User;
import org.springframework.security.web.authentication.AuthenticationSuccessHandler;
import org.springframework.stereotype.Component;

import com.example.repository.UserRepository;

import java.io.IOException;
@Component
public class GoogleOAuthSuccessHandler implements AuthenticationSuccessHandler {

    @Autowired
    private UserRepository userRepository;

    @Autowired
    private JwtUtil jwtUtil;

    @Override
    public void onAuthenticationSuccess(
            HttpServletRequest request,
            HttpServletResponse response,
            Authentication authentication
    ) throws IOException {

        OAuth2User user = (OAuth2User) authentication.getPrincipal();
        String email = user.getAttribute("email");

        boolean exists = userRepository.findByEmail(email).isPresent();

        if (exists) {
            String token = jwtUtil.generateToken(email);
            response.sendRedirect(
                "http://localhost:5173/oauth2/redirect?token=" + token
            );
        } else {
            response.sendRedirect(
                "http://localhost:5173/register?email=" + email
            );
        }
    }
}
