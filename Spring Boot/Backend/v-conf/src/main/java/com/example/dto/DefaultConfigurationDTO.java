package com.example.dto;

public class DefaultConfigurationDTO {

    private Integer id;
    private String name;
    private String compType;   // S / I / E / C

    public DefaultConfigurationDTO(
            Integer id,
            String name,
            String compType
    ) {
        this.id = id;
        this.name = name;
        this.compType = compType;
    }

    public Integer getId() {
        return id;
    }

    public String getName() {
        return name;
    }

    public String getCompType() {
        return compType;
    }
}
