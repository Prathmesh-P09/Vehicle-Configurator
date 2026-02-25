package com.example.logging;

import com.example.models.User;
import org.aspectj.lang.ProceedingJoinPoint;
import org.aspectj.lang.annotation.*;
import org.springframework.stereotype.Component;

import java.time.LocalDateTime;

@Aspect
@Component
public class RegistrationSecurityAspect {

    @Around("execution(* com.example.service.UserService.saveRegistration(..))")
    public Object encryptAndLog(ProceedingJoinPoint joinPoint) throws Throwable {

        User user = (User) joinPoint.getArgs()[0];
        String rawPassword = user.getPassword();

        // 🔐 Encrypt BEFORE DB save
        String encryptedPassword = BCryptUtil.hash(rawPassword);
        user.setPassword(encryptedPassword);

        try {
            Object result = joinPoint.proceed();

            // ✅ SUCCESS LOG
            RegistrationLogEntry log = new RegistrationLogEntry();
            log.timestamp = LocalDateTime.now();
            log.status = "SUCCESS";
            log.username = user.getUsername();
            log.email = user.getEmail();
            log.role = user.getRole();
            log.companyName = user.getCompanyName();
            log.registrationNo = user.getRegistrationNo();
            log.passwordHash = encryptedPassword;

            RegistrationFileLogger.log(log);
            return result;

        } catch (Exception ex) {

            // ❌ FAILURE LOG
            RegistrationLogEntry log = new RegistrationLogEntry();
            log.timestamp = LocalDateTime.now();
            log.status = "FAILED";	
            log.username = user.getUsername();
            log.email = user.getEmail();
            log.reason = ex.getMessage();

            RegistrationFileLogger.log(log);
            throw ex;
        }
    }
}
