package com.example.aop;

import java.util.Map;
import java.util.concurrent.ConcurrentHashMap;

import org.aspectj.lang.ProceedingJoinPoint;
import org.aspectj.lang.annotation.Around;
import org.aspectj.lang.annotation.Aspect;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Component;

@Aspect
@Component
public class WelcomeControllerTimingAspect {

    private static final Logger log =
            LoggerFactory.getLogger(WelcomeControllerTimingAspect.class);

    private final Map<String, Long> executionHistory = new ConcurrentHashMap<>();

    // 🔥 ONLY WelcomeController methods
    @Around("execution(* com.example.controller.WelcomeController.*(..))")
    public Object logFirstVsNextApiCall(ProceedingJoinPoint joinPoint) throws Throwable {

        String methodName = joinPoint.getSignature().toShortString();

        long start = System.nanoTime();
        Object result = joinPoint.proceed();
        long end = System.nanoTime();

        long timeMs = (end - start) / 1_000_000;

        if (!executionHistory.containsKey(methodName)) {
            log.info("🟡 FIRST API call {} took {} ms", methodName, timeMs);
        } else {
            long prev = executionHistory.get(methodName);
            log.info("🟢 NEXT API call {} took {} ms (previous: {} ms)",
                    methodName, timeMs, prev);
        }

        executionHistory.put(methodName, timeMs);

        return result;
    }
}
	