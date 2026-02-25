package com.example;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.boot.autoconfigure.domain.EntityScan;
import org.springframework.cache.annotation.EnableCaching;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.EnableAspectJAutoProxy;
import org.springframework.data.jpa.repository.config.EnableJpaRepositories;
import org.springframework.web.client.RestTemplate;

@SpringBootApplication
@EntityScan(basePackages = "com.example.models")
@EnableJpaRepositories(basePackages = "com.example.repository")
@EnableAspectJAutoProxy
@EnableCaching
public class VConfApplication {

	public static void main(String[] args) {
		SpringApplication.run(VConfApplication.class, args);
	}

	// ✅ REQUIRED for calling PDF microservice
	@Bean
	public RestTemplate restTemplate() {
		return new RestTemplate();
	}
}
