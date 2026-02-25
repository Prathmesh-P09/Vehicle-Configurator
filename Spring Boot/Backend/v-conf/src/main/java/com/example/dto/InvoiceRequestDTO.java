package com.example.dto;

import java.util.List;

public class InvoiceRequestDTO {

    private Integer modelId;
    private Integer qty;
    private String customerDetail;
    private List<AlternateComponentDTO> components;

    public Integer getModelId() {
        return modelId;
    }

    public void setModelId(Integer modelId) {
        this.modelId = modelId;
    }

    public Integer getQty() {
        return qty;
    }

    public void setQty(Integer qty) {
        this.qty = qty;
    }

    public String getCustomerDetail() {
        return customerDetail;
    }

    public void setCustomerDetail(String customerDetail) {
        this.customerDetail = customerDetail;
    }

    public List<AlternateComponentDTO> getComponents() {
        return components;
    }

    public void setComponents(List<AlternateComponentDTO> components) {
        this.components = components;
    }
}
