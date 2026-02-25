package com.example.repository;

import java.util.List;

import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;

import com.example.models.VehicleDefaultConfig;

public interface DefaultConfigRepository extends JpaRepository<VehicleDefaultConfig,Integer>{

	List<VehicleDefaultConfig> findByModel_Id(Integer id);


	
	
}
