package com.example.service;

import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import com.example.dto.ComponentDropdownDto;
import com.example.dto.OptionDto;
import com.example.models.Component;
import com.example.models.VehicleDetail;
import com.example.repository.VehicleDetailRepository;

@Service
public class VehicleManagerImpl implements VehicleManager {

    @Autowired
    private VehicleDetailRepository vehicleRepo;

    @Override
    public List<ComponentDropdownDto> getConfigurableComponents(Integer modelId, String compType) {

        List<VehicleDetail> details =
                vehicleRepo.findConfigurableComponents(modelId, compType);

        Map<Integer, ComponentDropdownDto> groupedMap = new LinkedHashMap<>();

        for (VehicleDetail vd : details) {

            Integer baseCompId = vd.getComp().getCompId(); // ✅ BASE COMPONENT
            String componentName = vd.getComp().getCompName();

            Component optionComp = vd.getComp(); // alternate component

            OptionDto option = new OptionDto(
                    optionComp.getCompId(),
                    optionComp.getType(),
                    optionComp.getPrice()
            );

            groupedMap
                .computeIfAbsent(
                    baseCompId,
                    id -> new ComponentDropdownDto(
                            id,
                            componentName,
                            new ArrayList<>()
                    )
                )
                .getOptions()
                .add(option);
        }

        return new ArrayList<>(groupedMap.values());
    }

}
