using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Transport
{
    [TestClass]
    public class FillDataBase
        : TestBase
    {
        private const int CUSTOMER_COUNT = 10_000;
        private const int MATERIAL_COUNT = 100_000;
        private const int OPERATION_COUNT = 100_000;
        private const int EMPLYEE_COUNT = 1_000;
        private const int ORDER_COUNT = 1_000_000;

        private List<Guid> _customers = new List<Guid>();
        private List<Guid> _materials = new List<Guid>();
        private List<Guid> _operations = new List<Guid>();
        private List<Guid> _emplyees = new List<Guid>();

        private Random _rnd = new Random();

        [TestMethod]
        public void FillBase()
        {
            for (int index = 0; index < CUSTOMER_COUNT; index++)
            {
                var customer = GenerateCustomer();
                _customers.Add(customer.Id);

                StoreCustomer(customer);
            }

            for (int index = 0; index < MATERIAL_COUNT; index++)
            {
                var material = GenerateMaterial();
                _materials.Add(material.Id);

                StoreMaterial(material);
            }

            for (int index = 0; index < OPERATION_COUNT; index++)
            {
                var operation = GenerateOperation();
                _operations.Add(operation.Id);

                StoreOperation(operation);
            }

            var post = GenerateAndStorePost();
            var group = GenerateAndStoreUserGroup();

            for (int index = 0; index < EMPLYEE_COUNT; index++)
            {
                var employee = GenerateEmployee(post.Id, group.Id);
                _emplyees.Add(employee.Id);

                StoreEmployee(employee);
            }

            for (int index = 0; index < ORDER_COUNT; index++)
            {
                var orderGroup = GenerateAndStoreOrderGroup();

                var customerId = _customers[_rnd.Next(0, CUSTOMER_COUNT - 1)];

                var order = GenerateAndStoreOrder(customerId, null, orderGroup.Id);

                var materialId = _materials[_rnd.Next(0, MATERIAL_COUNT - 1)];
                GenerateAndStoreLimitMaterial(materialId, order.Id);

                var operationId = _operations[_rnd.Next(0, OPERATION_COUNT - 1)];
                GenerateAndStoreLimitOperation(operationId, order.Id);

                var employeeId = _emplyees[_rnd.Next(0, EMPLYEE_COUNT - 1)];
                GenerateAndStorOrderOperation(operationId, employeeId, order.Id);
            }
        }
    }
}
