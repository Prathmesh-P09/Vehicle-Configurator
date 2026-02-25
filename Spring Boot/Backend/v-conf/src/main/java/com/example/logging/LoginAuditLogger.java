package com.example.logging;

import java.io.File;
import java.io.FileWriter;
import java.io.IOException;
import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;

public class LoginAuditLogger {

    private static final String LOG_DIR = "C:/logs";
    private static final String LOG_FILE = LOG_DIR + File.separator + "login-audit.log";

    static {
        File dir = new File(LOG_DIR);
        if (!dir.exists()) {
            dir.mkdirs();   // ✅ create C:/logs automatically
        }
    }

    public static synchronized void log(String message) {
        try (FileWriter fw = new FileWriter(LOG_FILE, true)) {
            fw.write(message + System.lineSeparator());
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public static String timestamp() {
        return LocalDateTime.now()
                .format(DateTimeFormatter.ofPattern("yyyy-MM-dd HH:mm:ss"));
    }
}
