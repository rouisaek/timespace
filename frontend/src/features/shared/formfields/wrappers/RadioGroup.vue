<template>
    <!-- Begin form field -->
    <div class="d-flex flex-column gap-1 h-100 mt-6">
        <label :for="name" :class="{ 'p-error': v$.modelValue.$invalid && showError, h6: true }">
            {{ label }}
        </label>
        <div class="d-flex flex-row gap-4 align-items-center" v-for="option in options" :key="option.value">
            <RadioButton :inputId="option.value" :value="option.value" v-model="v$.modelValue.$model"
                :class="componentClasses" />
            <label class="text-nowrap" @click="() => modelValue = option.value">
                {{ option.label }}
            </label>
        </div>
        <small :id="id + '-help'" v-if="helpText">{{ helpText }}<br></small>
        <span v-if="v$.modelValue.$invalid && showTextErrors">
            <span :id="id + '-error'" v-for="(error, index) of v$.modelValue.$errors" :key="index">
                <small class="p-error">{{ error.$message }}</small>
            </span>
        </span>
    </div>
    <!-- End form Field -->
</template>

<script lang="ts">
export default { inheritAttrs: false };
</script>

<script setup lang="ts">
import { useVuelidate } from "@vuelidate/core";
import { uniqueId } from "lodash-es";
import { computed, reactive, useAttrs } from "vue";
import { getRules, type RadioButtonGroupProps } from "../FormInputTypes";

const props = defineProps<RadioButtonGroupProps>();

const modelValue = defineModel();
const attrs = useAttrs();
const id = attrs["id"] as string ?? uniqueId("input");

const rules: any = getRules(props, false, props.label);

const v$ = useVuelidate({ modelValue: rules }, reactive({ modelValue }));

// eslint-disable-next-line vue/no-setup-props-destructure

const componentClasses = reactive<any>({
    "border-red-700 dark:border-red-300": computed(() => v$.value.modelValue.$invalid && props.showError),
});

v$.value.$touch();
</script>
