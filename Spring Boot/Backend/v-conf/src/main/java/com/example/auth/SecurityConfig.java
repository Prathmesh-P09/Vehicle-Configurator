package com.example.auth;

import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.security.authentication.AuthenticationManager;
import org.springframework.security.config.http.SessionCreationPolicy;
import org.springframework.security.web.SecurityFilterChain;
import org.springframework.security.web.authentication.UsernamePasswordAuthenticationFilter;
import org.springframework.web.servlet.config.annotation.CorsRegistry;
import org.springframework.web.servlet.config.annotation.WebMvcConfigurer;
import org.springframework.security.config.annotation.web.builders.HttpSecurity;

@Configuration
public class SecurityConfig {

    private final JwtAuthFilter jwtFilter;
    private final GoogleOAuthSuccessHandler googleOAuthSuccessHandler;

    public SecurityConfig(JwtAuthFilter jwtFilter, GoogleOAuthSuccessHandler googleOAuthSuccessHandler) {
        this.jwtFilter = jwtFilter;
        this.googleOAuthSuccessHandler = googleOAuthSuccessHandler;
    }

    @Bean
    public SecurityFilterChain filterChain(HttpSecurity http) throws Exception {

        http
        
            .csrf(csrf -> csrf.disable())
            .sessionManagement(sess ->
                sess.sessionCreationPolicy(SessionCreationPolicy.STATELESS)
            )
            .authorizeHttpRequests(auth -> auth
            	    .requestMatchers(
            	        "/actuator/**",
            	        "/auth/login",
            	        "/api/registration",
            	        "/api/registration/**",
            	        "/oauth2/**",
            	        "/login/oauth2/**"
            	    ).permitAll()
            	    .requestMatchers(
            	    		"/vehicle/**",
            	    		"/vehicaldetail/**",
                            "/api/welcome/**",
                            "/api/alternate-component/**",
                            "/api/default-config/**",
                            "/api/invoice/**"
            	    		 ).permitAll()
            	    .requestMatchers("/api/user/profile").permitAll()
            	    .requestMatchers("/api/invoice/**").authenticated()
            	    .anyRequest().authenticated()
            	)

            .oauth2Login(oauth -> oauth
                    .successHandler(googleOAuthSuccessHandler)
                )
            
            .addFilterBefore(jwtFilter, UsernamePasswordAuthenticationFilter.class);

        return http.build();
    }
    
    @Bean
    public WebMvcConfigurer corsConfigurer() {
        return new WebMvcConfigurer() {
            @Override
            public void addCorsMappings(CorsRegistry registry) {
                registry.addMapping("/**")
                    .allowedOrigins("http://localhost:5173")
                    .allowedMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
                    .allowedHeaders("*")
                    .allowCredentials(true);
            }
        };
    }

    
    @Bean
    public AuthenticationManager authenticationManager() {
        return authentication -> authentication;
    }
}
