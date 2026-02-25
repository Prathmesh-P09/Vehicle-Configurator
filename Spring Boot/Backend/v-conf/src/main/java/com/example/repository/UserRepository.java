package com.example.repository;

import java.util.Optional;

import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import com.example.models.User;

@Repository
public interface UserRepository extends JpaRepository<User, Integer> {

    Optional<User> findByUsername(String username);

    boolean existsByEmail(String email);
    Optional<User> findByEmail(String email);

    boolean existsByUsername(String username);

    boolean existsByPhone(String phone);
}