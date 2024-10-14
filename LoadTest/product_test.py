import base64
import random
import string
import uuid
import yaml
import logging
from locust import HttpUser, TaskSet, between, task

class ProductTask(TaskSet):
    
    def on_start(self):
        self.min_val = 5
        self.max_val = 15
        self.setup_resources()
        
    
    def setup_resources(self):
        self.categories = self.load_categories()
        self.generate_code() 
     
    def generate_code(self):
        guid = uuid.uuid4()
        guid_bytes = guid.bytes
        self.code = base64.urlsafe_b64encode(guid_bytes).decode('utf-8').rstrip('=')
   
    def load_categories(self):
        with open('./data/category.yml', 'r') as file:
            data = yaml.safe_load(file)  
        return data
        
    def random_text(self, length):
        characters = string.ascii_letters + string.digits + string.punctuation
        text = ''.join(random.choices(characters, k=length))
        return text
   
    def random_float(self,lower_bound, upper_bound):
       random_float = random.uniform(lower_bound, upper_bound)
       return random_float
        
    def random_category(self):
       category = random.choice(self.categories)
       category_name = category["category"]
       sub_categories = category["subcategories"]
       sub_category_name = random.choice(sub_categories)
       return (category_name, sub_category_name)
     
    def price(self):
       return self.min_val + (self.max_val - self.min_val) * random.random()


    def create_product(self):
        category = self.random_category()
        payload = {
            "code": self.code,
            "name": self.random_text(10),
            "description": self.random_text(50),
            "price": self.price(),
            "category": category[0],
            "subcategory": category[1],
            "brand": self.random_text(10),
            "size": self.random_float(1.0, 10.0),
            "weigh": self.random_float(100.0, 1000.0)
        }
        headers = {"Content-Type": "application/json"}
        response = self.client.post("/api/product", name="create product", json=payload, headers=headers)
        if response.status_code == 200:
            product_id = response.json()["id"]
            logging.info(f"Created product with ID: {product_id}")
            return product_id
        
        
    def delete_product(self, id):
        logging.info(f"Deleting product with ID: {id}")
        self.client.delete(f"/api/product/{id}", name="delete product",)
        
        
    def update_product(self, id):
        payload = {
            "name": self.random_text(10),
            "description": self.random_text(50),
        }

        headers = {"Content-Type": "application/json"}
        self.client.put(f"/api/product/{id}",name="update product", json=payload, headers=headers)
        
        
    def get_product(self, id):
        self.client.get(f"/api/product/{id}", name="get product")
        
    
    @task
    def update(self):
        product_id = self.create_product()
        self.update_product(product_id)
        self.delete_product(product_id)

    @task
    def search(self):
        product_id = self.create_product()
        self.get_product(product_id)
        self.delete_product(product_id)
            
    @task
    def delete(self):
        product_id = self.create_product()
        self.delete_product(product_id)
        


class LoadTesting(HttpUser):
    tasks = [ProductTask]
    wait_time = between(1, 5)
