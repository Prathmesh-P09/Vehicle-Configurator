package com.example.logging;

import java.io.File;
import java.io.FileWriter;
import java.time.format.DateTimeFormatter;

public class RegistrationFileLogger {

    private static final String LOG_DIR = "C:/logs";
    private static final String LOG_FILE = "registration-log.txt";

    public static synchronized void log(RegistrationLogEntry entry) {
        try {
            File dir = new File(LOG_DIR);
            if (!dir.exists()) dir.mkdirs();

            FileWriter fw = new FileWriter(new File(dir, LOG_FILE), true);

            fw.write("==================================================\n");
            fw.write("Timestamp        : " + entry.timestamp.format(
                    DateTimeFormatter.ofPattern("yyyy-MM-dd HH:mm:ss")) + "\n");
            fw.write("Status           : " + entry.status + "\n");

            if (entry.username != null)
                fw.write("Username         : " + entry.username + "\n");
            if (entry.email != null)
                fw.write("Email            : " + entry.email + "\n");
            if (entry.role != null)
                fw.write("Role             : " + entry.role + "\n");
            if (entry.companyName != null)
                fw.write("Company          : " + entry.companyName + "\n");
            if (entry.registrationNo != null)
                fw.write("Registration No  : " + entry.registrationNo + "\n");
            if (entry.passwordHash != null)
                fw.write("Password Hash    : " + entry.passwordHash + "\n");
            if (entry.reason != null)
                fw.write("Reason           : " + entry.reason + "\n");

            fw.write("==================================================\n\n");
            fw.close();

        } catch (Exception e) {
            e.printStackTrace();
        }
    }
}
