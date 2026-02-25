package com.example.dto;

import java.util.List;

public class ComponentDropdownDto {

    private Integer baseCompId;      // ✅ REQUIRED
    private String componentName;
    private List<OptionDto> options;

    // ✅ New constructor (recommended)
    public ComponentDropdownDto(Integer baseCompId, String componentName, List<OptionDto> options) {
        this.baseCompId = baseCompId;
        this.componentName = componentName;
        this.options = options;
    }

    // ✅ Optional: keep no-arg constructor if Jackson needs it
    public ComponentDropdownDto() {
    }

    public Integer getBaseCompId() {
        return baseCompId;
    }

    public void setBaseCompId(Integer baseCompId) {
        this.baseCompId = baseCompId;
    }

    public String getComponentName() {
        return componentName;
    }

    public void setComponentName(String componentName) {
        this.componentName = componentName;
    }

    public List<OptionDto> getOptions() {
        return options;
    }

    public void setOptions(List<OptionDto> options) {
        this.options = options;
    }
}
