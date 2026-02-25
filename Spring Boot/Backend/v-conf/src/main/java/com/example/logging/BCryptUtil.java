package com.example.logging;

import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;

public class BCryptUtil {

    private static final BCryptPasswordEncoder encoder =
            new BCryptPasswordEncoder(10);

    public static String hash(String rawPassword) {
        return encoder.encode(rawPassword);
    }

    // ✅ ADD THIS METHOD
    public static boolean matches(String rawPassword, String hashedPassword) {
        return encoder.matches(rawPassword, hashedPassword);
    }
}
