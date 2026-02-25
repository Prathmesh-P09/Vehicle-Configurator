package com.example.logging;

import org.aspectj.lang.ProceedingJoinPoint;
import org.aspectj.lang.annotation.Around;
import org.aspectj.lang.annotation.Aspect;
import org.springframework.stereotype.Component;
import com.example.dto.LoginRequest;
import jakarta.servlet.http.HttpServletRequest;
import org.springframework.web.context.request.RequestContextHolder;
import org.springframework.web.context.request.ServletRequestAttributes;

@Aspect
@Component
public class LoginAuditAspect {

    @Around("execution(* com.example.service.UserManager.login(..))")
    public Object logLoginAttempt(ProceedingJoinPoint joinPoint) throws Throwable {

        Object result;
        String username = "UNKNOWN";
        String ip = "UNKNOWN";

        try {
            Object[] args = joinPoint.getArgs();

if (args.length > 0 && args[0] instanceof LoginRequest req) {
    username = req.getUsername(); // OR req.getEmail()
}

            HttpServletRequest request =
                    ((ServletRequestAttributes) RequestContextHolder.currentRequestAttributes())
                            .getRequest();
            ip = request.getRemoteAddr();

            result = joinPoint.proceed();

            LoginAuditLogger.log(
                    LoginAuditLogger.timestamp() +
                    " | LOGIN_SUCCESS | user=" + username +
                    " | ip=" + ip
            );

            return result;

        } catch (Exception ex) {

            LoginAuditLogger.log(
                    LoginAuditLogger.timestamp() +
                    " | LOGIN_FAILED | user=" + username +
                    " | ip=" + ip +
                    " | reason=" + ex.getMessage()
            );

            throw ex;
        }
    }
}
